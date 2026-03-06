using System;
using System.Collections.Generic;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.InAppPurchase;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Services;
using _Project.Runtime.Views;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class ShopPresenter : IInitializable, IDisposable
    {
        private readonly SceneAssetProvider _assetProvider;
        private readonly MenuLoadingTasksProcessor _menuLoadingTasksProcessor;
        private readonly IResourcesService _resourcesService;
        private readonly IIapService _iapService;
        private ShopVisualCatalog _shopVisualCatalog;

        private readonly List<ShopProductCardData> _productsData = new();

        private ShopView _shopView;

        public ShopPresenter(SceneAssetProvider assetProvider,
            MenuLoadingTasksProcessor menuLoadingTasksProcessor,
            IResourcesService resourcesService,
            IIapService iapService)
        {
            _assetProvider = assetProvider;
            _menuLoadingTasksProcessor = menuLoadingTasksProcessor;
            _resourcesService = resourcesService;
            _iapService = iapService;
        }

        public void Initialize()
        {
            if (_menuLoadingTasksProcessor.IsFinished)
            {
                OnLoadingTasksFinished();
            }
            else
            {
                _menuLoadingTasksProcessor.OnTasksFinished += OnLoadingTasksFinished;
            }
        }

        public void Dispose()
        {
            _menuLoadingTasksProcessor.OnTasksFinished -= OnLoadingTasksFinished;
            _iapService.PurchasesChanged -= OnPurchasesChanged;

            if (_shopView)
            {
                _shopView.BackgroundClicked -= OnBackgroundClicked;
                _shopView.PurchaseConfirmed -= OnPurchaseConfirmed;
                _shopView.RestorePurchasesRequested -= OnRestorePurchasesRequested;
                _shopView.CloseRequested -= OnCloseRequested;
            }
        }

        public void OpenShop()
        {
            if (!_shopView)
            {
                Debug.LogWarning("[ShopPresenter] Shop view is not ready yet.");
                return;
            }

            FillProducts();
            _shopView.Show();
        }

        public void CloseShop()
        {
            if (_shopView)
            {
                _shopView.Hide();
            }
        }

        private void OnLoadingTasksFinished()
        {
            _menuLoadingTasksProcessor.OnTasksFinished -= OnLoadingTasksFinished;
            _iapService.PurchasesChanged -= OnPurchasesChanged;
            _iapService.PurchasesChanged += OnPurchasesChanged;

            if (!_assetProvider.TryGetLoadedComponent(out _shopView) || !_shopView)
            {
                Debug.LogError("[ShopPresenter] ShopView not provided by SceneAssetProvider.");
                return;
            }

            _shopView.BackgroundClicked += OnBackgroundClicked;
            _shopView.PurchaseConfirmed += OnPurchaseConfirmed;
            _shopView.RestorePurchasesRequested += OnRestorePurchasesRequested;
            _shopView.CloseRequested += OnCloseRequested;
            _shopView.Hide();
            TryResolveVisualCatalog();
            FillProducts();
        }

        private void OnBackgroundClicked()
        {
            CloseShop();
        }

        private void FillProducts()
        {
            if (!_shopView)
            {
                return;
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

            _shopView.SetProducts(_productsData);
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
                Debug.LogWarning($"[ShopPresenter] Failed to resolve ShopVisualCatalog from IResourcesService. {ex.Message}");
            }
        }

        private void OnPurchaseConfirmed(string productId)
        {
            _iapService.Purchase(productId);
            FillProducts();
        }

        private void OnPurchasesChanged()
        {
            FillProducts();
        }

        private void OnRestorePurchasesRequested()
        {
            if (!_iapService.RestoreTransactions())
            {
                Debug.LogWarning("[ShopPresenter] Failed to start restore transactions.");
            }
        }

        private void OnCloseRequested()
        {
            CloseShop();
        }
    }
}
