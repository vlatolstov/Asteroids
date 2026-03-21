using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace _Project.Runtime.Services
{
    public sealed class PlayerDataManager
    {
        private readonly ISaveService _localSaveService;
        private readonly ISaveService _cloudSaveService;
        private readonly PlayerModel _playerModel;

        private PlayerData _playerData;
        private string _playerId;
        private bool _missingPlayerIdLogged;

        public PlayerDataManager(
            [Inject(Id = SaveServiceId.Local)] ISaveService localSaveService,
            [Inject(Id = SaveServiceId.Cloud)] ISaveService cloudSaveService,
            PlayerModel playerModel)
        {
            _localSaveService = localSaveService;
            _cloudSaveService = cloudSaveService;
            _playerModel = playerModel;

            _playerData = Normalize(new PlayerData());
            _playerModel.Apply(_playerData);
        }

        private bool IsInitialized => !string.IsNullOrWhiteSpace(_playerId);

        public void InitializeForPlayer(string playerId, PlayerData initialData)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("PlayerId is null or empty.", nameof(playerId));
            }

            _playerId = playerId;
            _missingPlayerIdLogged = false;

            var normalized = Normalize(initialData ?? new PlayerData());
            _playerData = Clone(normalized);
            _playerModel.Apply(_playerData);

            UniTask.Void(() => SaveLocalSilentlyAsync(_playerData));
        }

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

        public void ClearLocalPlayerData()
        {
            Commit(new PlayerData(), forceSave: true);
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

        private bool Commit(PlayerData next, bool forceSave = false)
        {
            var normalized = Normalize(next);
            var changed = forceSave || !IsSamePayload(_playerData, normalized);
            if (!changed)
            {
                return false;
            }

            normalized.LastSavedAtUnixMs = NextTimestamp(_playerData?.LastSavedAtUnixMs ?? 0);

            if (IsInitialized)
            {
                UniTask.Void(() => SaveLocalSilentlyAsync(normalized));
                UniTask.Void(() => SaveCloudSilentlyAsync(normalized));
            }
            else if (!_missingPlayerIdLogged)
            {
                Debug.LogWarning("[PlayerData] PlayerId is not initialized yet, data changes are in-memory only.");
                _missingPlayerIdLogged = true;
            }

            _playerData = Clone(normalized);
            _playerModel.Apply(_playerData);
            return true;
        }

        private async UniTaskVoid SaveLocalSilentlyAsync(PlayerData data)
        {
            if (data == null)
            {
                return;
            }

            if (!await _localSaveService.Save(Clone(data)))
            {
                Debug.LogWarning("[PlayerData] Failed to persist player data locally.");
            }
        }

        private async UniTaskVoid SaveCloudSilentlyAsync(PlayerData data)
        {
            if (data == null)
            {
                return;
            }

            await _cloudSaveService.Save(Clone(data));
        }

        private static bool IsSamePayload(PlayerData left, PlayerData right)
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
                LastSavedAtUnixMs = Math.Max(0, source.LastSavedAtUnixMs),
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
            normalized.LastSavedAtUnixMs = Math.Max(0, normalized.LastSavedAtUnixMs);
            normalized.BestScore = Math.Max(0, normalized.BestScore);
            normalized.NonConsumableProductIds = NormalizeProductIds(normalized.NonConsumableProductIds);
            normalized.ActiveSubscriptionProductIds = NormalizeProductIds(normalized.ActiveSubscriptionProductIds);
            normalized.Consumables = NormalizeConsumables(normalized.Consumables);

            return normalized;
        }

        private static long NextTimestamp(long previousTimestamp)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return now > previousTimestamp ? now : previousTimestamp + 1;
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
