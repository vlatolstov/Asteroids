using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Project.Runtime.Services
{
    public sealed class PlayerDataManager
    {
        private readonly ISaveService _saveService;
        private readonly PlayerModel _playerModel;
        private PlayerData _playerData;

        public PlayerDataManager(ISaveService saveService, PlayerModel playerModel)
        {
            _saveService = saveService;
            _playerModel = playerModel;

            _playerData = LoadPlayerData();
            Commit(_playerData, forceSave: true);
        }

        public int BestScore => _playerModel.BestScore;

        public bool CheckEntitlement(string productId, ProductType productType)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            switch (productType)
            {
                case ProductType.Consumable:
                    return false;
                case ProductType.Subscription:
                    return _playerModel.HasActiveSubscription(productId);
                case ProductType.NonConsumable:
                    return _playerModel.HasNonConsumable(productId);
                default:
                    return _playerModel.HasNonConsumable(productId) ||
                           _playerModel.HasActiveSubscription(productId);
            }
        }

        public bool SetBestScore(int bestScore)
        {
            if (bestScore <= _playerData.BestScore)
            {
                return false;
            }

            var next = Clone(_playerData);
            next.BestScore = bestScore;
            return Commit(next);
        }

        public bool AddConsumable(string productId, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(productId) || amount <= 0)
            {
                return false;
            }

            var next = Clone(_playerData);
            var item = FindConsumable(next.Consumables, productId);
            if (item == null)
            {
                next.Consumables.Add(new ConsumableData
                {
                    ProductId = productId,
                    Amount = amount
                });
            }
            else
            {
                item.Amount += amount;
            }

            return Commit(next);
        }

        public bool RegisterPurchase(string productId, ProductType productType, bool includeConsumables = true)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            switch (productType)
            {
                case ProductType.Consumable:
                    return includeConsumables && AddConsumable(productId, 1);
                case ProductType.Subscription:
                    return AddUniqueProductId(productId, listSelector: data => data.ActiveSubscriptionProductIds);
                case ProductType.NonConsumable:
                case ProductType.Unknown:
                default:
                    return AddUniqueProductId(productId, listSelector: data => data.NonConsumableProductIds);
            }
        }

        public bool SyncEntitlements(IEnumerable<string> nonConsumableProductIds,
            IEnumerable<string> activeSubscriptionProductIds)
        {
            var next = Clone(_playerData);
            next.NonConsumableProductIds = NormalizeProductIds(nonConsumableProductIds);
            next.ActiveSubscriptionProductIds = NormalizeProductIds(activeSubscriptionProductIds);
            return Commit(next);
        }

        private bool AddUniqueProductId(string productId, Func<PlayerData, List<string>> listSelector)
        {
            var next = Clone(_playerData);
            var list = listSelector(next);
            if (list.Contains(productId))
            {
                return false;
            }

            list.Add(productId);
            return Commit(next);
        }

        private PlayerData LoadPlayerData()
        {
            if (_saveService.TryLoad(DataKeys.PLAYER_DATA_KEY, out PlayerData loadedData) && loadedData != null)
            {
                return Normalize(loadedData);
            }

            return Normalize(new PlayerData());
        }

        private bool Commit(PlayerData next, bool forceSave = false)
        {
            var normalized = Normalize(next);
            var changed = forceSave || !IsSame(_playerData, normalized);
            if (!changed)
            {
                return false;
            }

            if (!_saveService.Save(DataKeys.PLAYER_DATA_KEY, normalized))
            {
                Debug.LogWarning("[PlayerData] Failed to persist player data.");
            }

            _playerData = Clone(normalized);
            _playerModel.Apply(_playerData);
            return true;
        }

        private static bool IsSame(PlayerData left, PlayerData right)
        {
            left ??= new PlayerData();
            right ??= new PlayerData();

            if (left.BestScore != right.BestScore)
            {
                return false;
            }

            if (!left.NonConsumableProductIds.SequenceEqual(right.NonConsumableProductIds))
            {
                return false;
            }

            if (!left.ActiveSubscriptionProductIds.SequenceEqual(right.ActiveSubscriptionProductIds))
            {
                return false;
            }

            if (left.Consumables.Count != right.Consumables.Count)
            {
                return false;
            }

            for (var i = 0; i < left.Consumables.Count; i++)
            {
                var leftItem = left.Consumables[i];
                var rightItem = right.Consumables[i];
                if (leftItem.ProductId != rightItem.ProductId || leftItem.Amount != rightItem.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        private static PlayerData Clone(PlayerData source)
        {
            source ??= new PlayerData();

            var clone = new PlayerData
            {
                BestScore = source.BestScore,
                NonConsumableProductIds = new List<string>(source.NonConsumableProductIds ?? new List<string>()),
                ActiveSubscriptionProductIds = new List<string>(source.ActiveSubscriptionProductIds ?? new List<string>()),
                Consumables = new List<ConsumableData>()
            };

            var consumables = source.Consumables ?? new List<ConsumableData>();
            for (var i = 0; i < consumables.Count; i++)
            {
                var item = consumables[i];
                if (item == null)
                {
                    continue;
                }

                clone.Consumables.Add(new ConsumableData
                {
                    ProductId = item.ProductId,
                    Amount = item.Amount
                });
            }

            return clone;
        }

        private static PlayerData Normalize(PlayerData data)
        {
            data ??= new PlayerData();

            var normalized = Clone(data);
            normalized.BestScore = Math.Max(0, normalized.BestScore);
            normalized.NonConsumableProductIds = NormalizeProductIds(normalized.NonConsumableProductIds);
            normalized.ActiveSubscriptionProductIds = NormalizeProductIds(normalized.ActiveSubscriptionProductIds);
            normalized.Consumables = NormalizeConsumables(normalized.Consumables);

            return normalized;
        }

        private static List<string> NormalizeProductIds(IEnumerable<string> source)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            if (source != null)
            {
                foreach (var productId in source)
                {
                    if (string.IsNullOrWhiteSpace(productId))
                    {
                        continue;
                    }

                    result.Add(productId);
                }
            }

            var sorted = result.ToList();
            sorted.Sort(StringComparer.Ordinal);
            return sorted;
        }

        private static List<ConsumableData> NormalizeConsumables(IEnumerable<ConsumableData> source)
        {
            var byProductId = new Dictionary<string, int>(StringComparer.Ordinal);
            if (source != null)
            {
                foreach (var item in source)
                {
                    if (item == null || string.IsNullOrWhiteSpace(item.ProductId) || item.Amount <= 0)
                    {
                        continue;
                    }

                    if (byProductId.TryGetValue(item.ProductId, out var amount))
                    {
                        byProductId[item.ProductId] = amount + item.Amount;
                    }
                    else
                    {
                        byProductId[item.ProductId] = item.Amount;
                    }
                }
            }

            var normalized = byProductId
                .Select(pair => new ConsumableData
                {
                    ProductId = pair.Key,
                    Amount = pair.Value
                })
                .ToList();

            normalized.Sort((a, b) => string.CompareOrdinal(a.ProductId, b.ProductId));
            return normalized;
        }

        private static ConsumableData FindConsumable(IList<ConsumableData> source, string productId)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (item != null && item.ProductId == productId)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
