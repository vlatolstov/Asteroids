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
        public event Action<int> BestScoreChanged;
        
        public void Apply(PlayerData data)
        {
            data ??= new PlayerData();

            var previousBestScore = BestScore;

            BestScore = Math.Max(0, data.BestScore);

            _nonConsumableProductIds.Clear();
            AddProductIds(_nonConsumableProductIds, data.NonConsumableProductIds);

            _activeSubscriptionProductIds.Clear();
            AddProductIds(_activeSubscriptionProductIds, data.ActiveSubscriptionProductIds);

            _consumablesByProductId.Clear();
            if (data.Consumables != null)
            {
                foreach (var item in data.Consumables)
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

            if (BestScore != previousBestScore)
            {
                BestScoreChanged?.Invoke(BestScore);
            }
        }

        public bool HasNonConsumable(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && _nonConsumableProductIds.Contains(productId);
        }

        public bool HasActiveSubscription(string productId)
        {
            return !string.IsNullOrWhiteSpace(productId) && _activeSubscriptionProductIds.Contains(productId);
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
    }
}
