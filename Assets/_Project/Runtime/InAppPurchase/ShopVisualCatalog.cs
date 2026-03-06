using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Runtime.InAppPurchase
{
    [CreateAssetMenu(fileName = "ShopVisualCatalog", menuName = "IAP/Shop Visual Catalog")]
    public class ShopVisualCatalog : ScriptableObject
    {
        [SerializeField] private List<ShopVisualEntry> entries = new();

        private Dictionary<string, Sprite> _iconsByProductId;

        [field: SerializeField] public Sprite DefaultIcon { get; private set; }

        public Sprite GetIconOrDefault(string productId)
        {
            if (TryGetIcon(productId, out var icon))
            {
                return icon;
            }

            return DefaultIcon;
        }

        private bool TryGetIcon(string productId, out Sprite icon)
        {
            icon = null;

            if (string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            EnsureLookup();

            return _iconsByProductId.TryGetValue(productId, out icon) && icon;
        }

        private void EnsureLookup()
        {
            if (_iconsByProductId != null)
            {
                return;
            }

            _iconsByProductId = new Dictionary<string, Sprite>();

            foreach (var entry in entries)
            {
                if (entry == null ||
                    string.IsNullOrWhiteSpace(entry.ProductId) ||
                    _iconsByProductId.ContainsKey(entry.ProductId))
                {
                    continue;
                }

                _iconsByProductId.Add(entry.ProductId, entry.Icon);
            }
        }

        private void OnValidate()
        {
            _iconsByProductId = null;
        }
    }

    [Serializable]
    public class ShopVisualEntry
    {
        [field: SerializeField]
        public string ProductId { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
    }
}