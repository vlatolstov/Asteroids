using UnityEngine;

namespace _Project.Runtime.InAppPurchase
{
    public sealed class ShopProductCardData
    {
        public ShopProductCardData(string productId, string title, string description, string price, Sprite icon,
            bool isPurchased)
        {
            ProductId = productId;
            Title = title;
            Description = description;
            Price = price;
            Icon = icon;
            IsPurchased = isPurchased;
        }

        public string ProductId { get; }
        public string Title { get; }
        public string Description { get; }
        public string Price { get; }
        public Sprite Icon { get; }
        public bool IsPurchased { get; }
    }
}
