using System;
using _Project.Runtime.Constants;
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

        private MenuView _view;

        public MenuPresenter(ViewsContainer viewsContainer,
            SceneLoader sceneLoader,
            BestScoreService bestScoreService)
        {
            _viewsContainer = viewsContainer;
            _sceneLoader = sceneLoader;
            _bestScoreService = bestScoreService;
        }

        public void Initialize()
        {
            _view = _viewsContainer.GetView<MenuView>();

            if (_view == null)
            {
                Debug.LogError("[MenuPresenter] MenuView was not found in the ViewsContainer.");
                return;
            }

            _view.StartButtonClicked += OnStartClicked;
            _view.ExitButtonClicked += OnExitClicked;
            _view.SetBestScore(_bestScoreService.Value);
        }

        public void Dispose()
        {
            if (_view == null)
            {
                return;
            }

            _view.StartButtonClicked -= OnStartClicked;
            _view.ExitButtonClicked -= OnExitClicked;
        }

        private void OnStartClicked()
        {
            UniTask.Void(async () =>
            {
                await _sceneLoader.LoadSceneAsync(Constants.Scenes.Game);
            });
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
