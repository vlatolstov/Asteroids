using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Runtime.Data
{
    [Serializable]
    public class PlayerData
    {
        public long LastSavedAtUnixMs;
        public int BestScore;
        public List<string> NonConsumableProductIds = new();
        public List<string> ActiveSubscriptionProductIds = new();
        public List<ConsumableData> Consumables = new();

        public bool IsSamePayload(PlayerData other)
        {
            other ??= new PlayerData();

            if (BestScore != other.BestScore ||
                !NonConsumableProductIds.SequenceEqual(other.NonConsumableProductIds) ||
                !ActiveSubscriptionProductIds.SequenceEqual(other.ActiveSubscriptionProductIds) ||
                Consumables.Count != other.Consumables.Count)
            {
                return false;
            }

            for (var i = 0; i < Consumables.Count; i++)
            {
                var a = Consumables[i];
                var b = other.Consumables[i];
                if (a.ProductId != b.ProductId || a.Amount != b.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        public PlayerData Clone()
        {
            var clone = new PlayerData
            {
                LastSavedAtUnixMs = LastSavedAtUnixMs,
                BestScore = BestScore,
                NonConsumableProductIds = new List<string>(NonConsumableProductIds ?? new List<string>()),
                ActiveSubscriptionProductIds = new List<string>(ActiveSubscriptionProductIds ?? new List<string>()),
                Consumables = new List<ConsumableData>()
            };

            var consumables = Consumables ?? new List<ConsumableData>();
            foreach (var item in consumables)
            {
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

        public void Normalize()
        {
            LastSavedAtUnixMs = Math.Max(0, LastSavedAtUnixMs);
            BestScore = Math.Max(0, BestScore);
            NonConsumableProductIds = NormalizeProductIds(NonConsumableProductIds);
            ActiveSubscriptionProductIds = NormalizeProductIds(ActiveSubscriptionProductIds);
            Consumables = NormalizeConsumables(Consumables);
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
    }

    [Serializable]
    public class ConsumableData
    {
        public string ProductId;
        public int Amount;
    }
}
