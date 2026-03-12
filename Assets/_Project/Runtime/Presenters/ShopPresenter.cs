using System;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class ShopPresenter : IInitializable, IDisposable
    {
        private readonly SceneAssetProvider _assetProvider;
        private readonly MenuLoadingTasksProcessor _menuLoadingTasksProcessor;
        private readonly ShopModel _shopModel;

        private ShopView _shopView;

        public ShopPresenter(SceneAssetProvider assetProvider,
            MenuLoadingTasksProcessor menuLoadingTasksProcessor,
            ShopModel shopModel)
        {
            _assetProvider = assetProvider;
            _menuLoadingTasksProcessor = menuLoadingTasksProcessor;
            _shopModel = shopModel;
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
            _shopModel.ProductsChanged -= OnProductsChanged;
            _shopModel.Dispose();

            if (_shopView)
            {
                _shopView.BackgroundClicked -= OnBackgroundClicked;
                _shopView.PurchaseConfirmed -= OnPurchaseConfirmed;
                _shopView.RestorePurchasesRequested -= OnRestorePurchasesRequested;
                _shopView.CloseRequested -= OnCloseRequested;
                _shopView.ClearPlayerDataRequested -= OnClearPlayerDataRequested;
            }
        }

        public void OpenShop()
        {
            if (!_shopView)
            {
                Debug.LogWarning("[ShopPresenter] Shop view is not ready yet.");
                return;
            }

            OnProductsChanged();
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

            if (!_assetProvider.TryGetLoadedComponent(out _shopView) || !_shopView)
            {
                Debug.LogError("[ShopPresenter] ShopView not provided by SceneAssetProvider.");
                return;
            }

            _shopView.BackgroundClicked += OnBackgroundClicked;
            _shopView.PurchaseConfirmed += OnPurchaseConfirmed;
            _shopView.RestorePurchasesRequested += OnRestorePurchasesRequested;
            _shopView.CloseRequested += OnCloseRequested;
            _shopView.ClearPlayerDataRequested += OnClearPlayerDataRequested;
            _shopModel.ProductsChanged += OnProductsChanged;
            _shopView.Hide();
            _shopModel.Initialize();
        }

        private void OnBackgroundClicked()
        {
            CloseShop();
        }

        private void OnProductsChanged()
        {
            if (!_shopView)
            {
                return;
            }

            _shopView.SetProducts(_shopModel.Products);
        }

        private void OnPurchaseConfirmed(string productId)
        {
            _shopModel.Purchase(productId);
        }

        private void OnRestorePurchasesRequested()
        {
            _shopModel.RestorePurchases();
        }

        private void OnCloseRequested()
        {
            CloseShop();
        }

        private void OnClearPlayerDataRequested()
        {
            _shopModel.ClearPlayerData();
        }
    }
}
