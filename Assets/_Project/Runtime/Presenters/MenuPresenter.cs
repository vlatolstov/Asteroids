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

        private MenuView _menuView;

        public MenuPresenter(SceneLoader sceneLoader,
            BestScoreService bestScoreService,
            MenuLoadingTasksProcessor menuLoadingTasksProcessor, SceneAssetProvider assetProvider)
        {
            _sceneLoader = sceneLoader;
            _bestScoreService = bestScoreService;
            _menuLoadingTasksProcessor = menuLoadingTasksProcessor;
            _assetProvider = assetProvider;
        }

        public void Initialize()
        {
            _menuLoadingTasksProcessor.OnTasksFinished += OnLoadingTaskFinished;
        }

        public void Dispose()
        {
            if (_menuView)
            {
                _menuView.StartButtonClicked -= OnStartClicked;
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
            _menuView.ExitButtonClicked += OnExitClicked;
            _menuView.SetBestScore(_bestScoreService.Value);

            _menuLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;
        }

        private void OnStartClicked()
        {
            UniTask.Void(async () => { await _sceneLoader.LoadSceneAsync(Scenes.Game); });
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