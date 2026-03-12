using System;
using System.Collections.Generic;
using _Project.Runtime.InAppPurchase;
using _Project.Runtime.Services;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Project.Runtime.Models
{
    public class ShopModel : IDisposable
    {
        private readonly IResourcesService _resourcesService;
        private readonly IIapService _iapService;
        private readonly PlayerDataManager _playerDataManager;

        private readonly List<ShopProductCardData> _productsData = new();
        private ShopVisualCatalog _shopVisualCatalog;

        public event Action ProductsChanged;
        public IReadOnlyList<ShopProductCardData> Products => _productsData;

        public ShopModel(IResourcesService resourcesService, IIapService iapService, PlayerDataManager playerDataManager)
        {
            _resourcesService = resourcesService;
            _iapService = iapService;
            _playerDataManager = playerDataManager;
        }

        public void Initialize()
        {
            _iapService.PurchasesChanged += OnPurchasesChanged;
            TryResolveVisualCatalog();
            RefreshProducts();
        }

        public void Dispose()
        {
            _iapService.PurchasesChanged -= OnPurchasesChanged;
        }

        public void Purchase(string productId)
        {
            _iapService.Purchase(productId);
            RefreshProducts();
        }

        public void RestorePurchases()
        {
            if (!_iapService.RestoreTransactions())
            {
                Debug.LogWarning("[ShopModel] Failed to start restore transactions.");
            }
        }

        public void ClearPlayerData()
        {
            _playerDataManager.ClearLocalPlayerData();
            RefreshProducts();
        }

        private void RefreshProducts()
        {
            if (!_shopVisualCatalog)
            {
                TryResolveVisualCatalog();
            }

            _productsData.Clear();

            var products = _iapService.Products;
            if (products != null)
            {
                for (var i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    if (product == null)
                    {
                        continue;
                    }

                    _productsData.Add(BuildCardData(product));
                }
            }

            ProductsChanged?.Invoke();
        }

        private ShopProductCardData BuildCardData(Product product)
        {
            var productId = product.definition?.id ?? string.Empty;
            var metadata = product.metadata;

            var title = !string.IsNullOrWhiteSpace(metadata?.localizedTitle)
                ? metadata.localizedTitle
                : productId;
            var description = metadata?.localizedDescription ?? string.Empty;
            var price = metadata?.localizedPriceString ?? string.Empty;
            var icon = _shopVisualCatalog ? _shopVisualCatalog.GetIconOrDefault(productId) : null;
            var isPurchased = _iapService.CheckEntitlement(productId);

            return new ShopProductCardData(productId, title, description, price, icon, isPurchased);
        }

        private void TryResolveVisualCatalog()
        {
            try
            {
                _shopVisualCatalog = _resourcesService.Get<ShopVisualCatalog>();
            }
            catch (Exception ex)
            {
                _shopVisualCatalog = null;
                Debug.LogWarning($"[ShopModel] Failed to resolve ShopVisualCatalog from IResourcesService. {ex.Message}");
            }
        }

        private void OnPurchasesChanged()
        {
            RefreshProducts();
        }
    }
}
