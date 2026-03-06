using System;
using System.Collections.Generic;

namespace _Project.Runtime.Data
{
    [Serializable]
    public class PlayerData
    {
        public int BestScore;
        public List<string> NonConsumableProductIds = new();
        public List<string> ActiveSubscriptionProductIds = new();
        public List<ConsumableData> Consumables = new();
    }

    [Serializable]
    public class ConsumableData
    {
        public string ProductId;
        public int Amount;
    }
}
