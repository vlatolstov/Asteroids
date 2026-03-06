using System;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Score;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class MenuPresenter : IInitializable, IDisposable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly BestScoreService _bestScoreService;
        private readonly MenuLoadingTasksProcessor _menuLoadingTasksProcessor;
        private readonly SceneAssetProvider _assetProvider;
        private readonly ShopPresenter _shopPresenter;

        private MenuView _menuView;

        public MenuPresenter(SceneLoader sceneLoader,
            BestScoreService bestScoreService,
            MenuLoadingTasksProcessor menuLoadingTasksProcessor,
            SceneAssetProvider assetProvider,
            ShopPresenter shopPresenter)
        {
            _sceneLoader = sceneLoader;
            _bestScoreService = bestScoreService;
            _menuLoadingTasksProcessor = menuLoadingTasksProcessor;
            _assetProvider = assetProvider;
            _shopPresenter = shopPresenter;
        }

        public void Initialize()
        {
            if (_menuLoadingTasksProcessor.IsFinished)
            {
                OnLoadingTaskFinished();
            }
            else
            {
                _menuLoadingTasksProcessor.OnTasksFinished += OnLoadingTaskFinished;
            }
        }

        public void Dispose()
        {
            if (_menuView)
            {
                _menuView.StartButtonClicked -= OnStartClicked;
                _menuView.ShopButtonClicked -= OnShopClicked;
                _menuView.ExitButtonClicked -= OnExitClicked;
            }
        }

        private void OnLoadingTaskFinished()
        {
            if (!_assetProvider.TryGetLoadedComponent(out _menuView) ||
                !_menuView)
            {
                Debug.LogError("MenuView not provided");
                return;
            }

            _menuView.StartButtonClicked += OnStartClicked;
            _menuView.ShopButtonClicked += OnShopClicked;
            _menuView.ExitButtonClicked += OnExitClicked;
            _menuView.SetBestScore(_bestScoreService.Value);

            _menuLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;
        }

        private void OnStartClicked()
        {
            UniTask.Void(async () => { await _sceneLoader.LoadSceneAsync(Scenes.Game); });
        }

        private void OnShopClicked()
        {
            _shopPresenter.OpenShop();
        }

        private void OnExitClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
