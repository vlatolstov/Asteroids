using System;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class ShopPresenter : IInitializable, IDisposable
    {
        private readonly ShopView _shopView;

        public ShopPresenter(ShopView shopView)
        {
            _shopView = shopView;
        }

        public void Initialize()
        {
            if (!_shopView)
            {
                Debug.LogError("[ShopPresenter] ShopView not found in scene.");
                return;
            }

            _shopView.BackgroundClicked += OnBackgroundClicked;
            _shopView.Hide();
        }

        public void Dispose()
        {
            if (_shopView)
            {
                _shopView.BackgroundClicked -= OnBackgroundClicked;
            }
        }

        public void OpenShop()
        {
            if (!_shopView)
            {
                Debug.LogError("[ShopPresenter] ShopView not found in scene.");
                return;
            }

            _shopView.Show();
        }

        public void CloseShop()
        {
            if (_shopView)
            {
                _shopView.Hide();
            }
        }

        private void OnBackgroundClicked()
        {
            CloseShop();
        }
    }
}
