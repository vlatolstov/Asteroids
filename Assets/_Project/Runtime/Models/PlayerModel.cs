using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using UnityEngine.Purchasing;

namespace _Project.Runtime.Models
{
    public sealed class PlayerModel
    {
        private readonly HashSet<string> _nonConsumableProductIds = new(StringComparer.Ordinal);
        private readonly HashSet<string> _activeSubscriptionProductIds = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _consumablesByProductId = new(StringComparer.Ordinal);

        public int BestScore { get; private set; }
        public event Action<int> BestScoreChanged;
        public event Action StateChanged;

        public void LoadSnapshot(PlayerData data)
        {
            data.Normalize();
            ApplySnapshot(data, notifyStateChanged: false);
        }

        public PlayerData CreateSnapshot()
        {
            var snapshotData = new PlayerData
            {
                BestScore = Math.Max(0, BestScore),
                NonConsumableProductIds = new List<string>(_nonConsumableProductIds),
                ActiveSubscriptionProductIds = new List<string>(_activeSubscriptionProductIds),
                Consumables = BuildRawConsumablesSnapshot()
            };
            
            snapshotData.Normalize();
            return snapshotData;
        }

        public bool TrySetBestScore(int bestScore)
        {
            if (bestScore <= BestScore)
            {
                return false;
            }

            BestScore = bestScore;
            BestScoreChanged?.Invoke(BestScore);
            StateChanged?.Invoke();
            return true;
        }

        public bool TryRegisterPurchase(string productId, ProductType productType, bool includeConsumables = true)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            switch (productType)
            {
                case ProductType.Consumable:
                    return includeConsumables && TryAddConsumable(productId, 1);
                case ProductType.Subscription:
                    return TryAddUniqueProductId(_activeSubscriptionProductIds, productId);
                case ProductType.NonConsumable:
                case ProductType.Unknown:
                default:
                    return TryAddUniqueProductId(_nonConsumableProductIds, productId);
            }
        }

        public bool ReplaceEntitlements(IEnumerable<string> nonConsumableProductIds,
            IEnumerable<string> activeSubscriptionProductIds)
        {
            var nextNonConsumables = CreateNormalizedProductIdSet(nonConsumableProductIds);
            var nextSubscriptions = CreateNormalizedProductIdSet(activeSubscriptionProductIds);

            if (_nonConsumableProductIds.SetEquals(nextNonConsumables) &&
                _activeSubscriptionProductIds.SetEquals(nextSubscriptions))
            {
                return false;
            }

            _nonConsumableProductIds.Clear();
            foreach (var productId in nextNonConsumables)
            {
                _nonConsumableProductIds.Add(productId);
            }

            _activeSubscriptionProductIds.Clear();
            foreach (var productId in nextSubscriptions)
            {
                _activeSubscriptionProductIds.Add(productId);
            }

            StateChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            ApplySnapshot(new PlayerData(), notifyStateChanged: true);
        }

        public bool CheckEntitlement(string productId, ProductType productType)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            return productType switch
            {
                ProductType.Consumable => false,
                ProductType.Subscription => HasActiveSubscription(productId),
                ProductType.NonConsumable => HasNonConsumable(productId),
                _ => HasNonConsumable(productId) || HasActiveSubscription(productId)
            };
        }

        public bool HasNonConsumable(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && _nonConsumableProductIds.Contains(productId);
        }

        public bool HasActiveSubscription(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && _activeSubscriptionProductIds.Contains(productId);
        }

        private void ApplySnapshot(PlayerData data, bool notifyStateChanged)
        {
            var current = CreateSnapshot();
            var payloadChanged = !current.IsSamePayload(data);
            var previousBestScore = BestScore;

            BestScore = Math.Max(0, data.BestScore);

            _nonConsumableProductIds.Clear();
            AddProductIds(_nonConsumableProductIds, data.NonConsumableProductIds);

            _activeSubscriptionProductIds.Clear();
            AddProductIds(_activeSubscriptionProductIds, data.ActiveSubscriptionProductIds);

            _consumablesByProductId.Clear();
            AddConsumables(data.Consumables);

            if (BestScore != previousBestScore)
            {
                BestScoreChanged?.Invoke(BestScore);
            }

            if (notifyStateChanged && payloadChanged)
            {
                StateChanged?.Invoke();
            }
        }

        private bool TryAddConsumable(string productId, int amount)
        {
            if (string.IsNullOrWhiteSpace(productId) || amount <= 0)
            {
                return false;
            }

            if (_consumablesByProductId.TryGetValue(productId, out var currentAmount))
            {
                _consumablesByProductId[productId] = currentAmount + amount;
            }
            else
            {
                _consumablesByProductId[productId] = amount;
            }

            StateChanged?.Invoke();
            return true;
        }

        private bool TryAddUniqueProductId(ISet<string> target, string productId)
        {
            if (string.IsNullOrWhiteSpace(productId) || !target.Add(productId))
            {
                return false;
            }

            StateChanged?.Invoke();
            return true;
        }

        private List<ConsumableData> BuildRawConsumablesSnapshot()
        {
            var consumables = new List<ConsumableData>();
            foreach (var pair in _consumablesByProductId)
            {
                consumables.Add(new ConsumableData
                {
                    ProductId = pair.Key,
                    Amount = pair.Value
                });
            }

            return consumables;
        }

        private static void AddProductIds(HashSet<string> target, IList<string> source)
        {
            if (source == null)
            {
                return;
            }

            foreach (var productId in source)
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    continue;
                }

                target.Add(productId);
            }
        }

        private static HashSet<string> CreateNormalizedProductIdSet(IEnumerable<string> source)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            if (source == null)
            {
                return result;
            }

            foreach (var productId in source)
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    continue;
                }

                result.Add(productId);
            }

            return result;
        }

        private void AddConsumables(IList<ConsumableData> consumables)
        {
            if (consumables == null)
            {
                return;
            }

            foreach (var item in consumables)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.ProductId) || item.Amount <= 0)
                {
                    continue;
                }

                if (_consumablesByProductId.TryGetValue(item.ProductId, out var currentAmount))
                {
                    _consumablesByProductId[item.ProductId] = currentAmount + item.Amount;
                }
                else
                {
                    _consumablesByProductId[item.ProductId] = item.Amount;
                }
            }
        }
    }
}
