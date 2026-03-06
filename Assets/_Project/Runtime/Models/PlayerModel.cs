using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Runtime.Data;

namespace _Project.Runtime.Models
{
    public sealed class PlayerModel
    {
        private readonly HashSet<string> _nonConsumableProductIds = new(StringComparer.Ordinal);
        private readonly HashSet<string> _activeSubscriptionProductIds = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _consumablesByProductId = new(StringComparer.Ordinal);

        public int BestScore { get; private set; }

        public event Action Changed;

        public void Apply(PlayerData data)
        {
            data ??= new PlayerData();

            BestScore = Math.Max(0, data.BestScore);

            _nonConsumableProductIds.Clear();
            AddProductIds(_nonConsumableProductIds, data.NonConsumableProductIds);

            _activeSubscriptionProductIds.Clear();
            AddProductIds(_activeSubscriptionProductIds, data.ActiveSubscriptionProductIds);

            _consumablesByProductId.Clear();
            if (data.Consumables != null)
            {
                for (var i = 0; i < data.Consumables.Count; i++)
                {
                    var item = data.Consumables[i];
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

            Changed?.Invoke();
        }

        public bool HasNonConsumable(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && _nonConsumableProductIds.Contains(productId);
        }

        public bool HasActiveSubscription(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && _activeSubscriptionProductIds.Contains(productId);
        }

        public int GetConsumableAmount(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return 0;
            }

            return _consumablesByProductId.TryGetValue(productId, out var amount) ? amount : 0;
        }

        public PlayerData Snapshot()
        {
            var nonConsumables = _nonConsumableProductIds.ToList();
            nonConsumables.Sort(StringComparer.Ordinal);

            var subscriptions = _activeSubscriptionProductIds.ToList();
            subscriptions.Sort(StringComparer.Ordinal);

            var consumables = new List<ConsumableData>(_consumablesByProductId.Count);
            foreach (var pair in _consumablesByProductId)
            {
                if (pair.Value <= 0)
                {
                    continue;
                }

                consumables.Add(new ConsumableData
                {
                    ProductId = pair.Key,
                    Amount = pair.Value
                });
            }

            consumables.Sort((a, b) => string.CompareOrdinal(a.ProductId, b.ProductId));

            return new PlayerData
            {
                BestScore = BestScore,
                NonConsumableProductIds = nonConsumables,
                ActiveSubscriptionProductIds = subscriptions,
                Consumables = consumables
            };
        }

        private static void AddProductIds(HashSet<string> target, IList<string> source)
        {
            if (source == null)
            {
                return;
            }

            for (var i = 0; i < source.Count; i++)
            {
                var productId = source[i];
                if (string.IsNullOrWhiteSpace(productId))
                {
                    continue;
                }

                target.Add(productId);
            }
        }
    }
}
