using System;
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
        private readonly ViewsContainer _viewsContainer;
        private readonly SceneLoader _sceneLoader;
        private readonly BestScoreService _bestScoreService;
        private readonly MenuLoadingTaskService _menuLoadingTaskService;

        private MenuView _view;

        public MenuPresenter(ViewsContainer viewsContainer,
            SceneLoader sceneLoader,
            BestScoreService bestScoreService,
            MenuLoadingTaskService menuLoadingTaskService)
        {
            _viewsContainer = viewsContainer;
            _sceneLoader = sceneLoader;
            _bestScoreService = bestScoreService;
            _menuLoadingTaskService = menuLoadingTaskService;
        }

        public void Initialize()
        {
            _menuLoadingTaskService.OnTasksFinished += OnLoadingTaskFinished;
        }

        public void Dispose()
        {
            if (!_view)
            {
                return;
            }

            _viewsContainer.RemoveView(_view);
            _view.StartButtonClicked -= OnStartClicked;
            _view.ExitButtonClicked -= OnExitClicked;
        }

        private void OnLoadingTaskFinished()
        {
            _view = _viewsContainer.GetView<MenuView>();

            if (_view)
            {
                _view.StartButtonClicked += OnStartClicked;
                _view.ExitButtonClicked += OnExitClicked;
                _view.SetBestScore(_bestScoreService.Value);
            }
            else
            {
                Debug.LogError("MenuView not provided");
            }

            _menuLoadingTaskService.OnTasksFinished -= OnLoadingTaskFinished;
        }

        private void OnStartClicked()
        {
            UniTask.Void(async () => { await _sceneLoader.LoadSceneAsync(Constants.Scenes.Game); });
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