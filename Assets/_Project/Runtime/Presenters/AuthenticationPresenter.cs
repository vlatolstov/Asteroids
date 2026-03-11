using System;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public sealed class AuthenticationPresenter : IInitializable, IDisposable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly SceneAssetProvider _assetProvider;
        private readonly AuthenticationLoadingTasksProcessor _loadingTasksProcessor;
        private readonly AuthenticationModel _authenticationModel;

        private AuthView _authView;
        private bool _navigationStarted;

        public AuthenticationPresenter(SceneLoader sceneLoader, SceneAssetProvider assetProvider,
            AuthenticationLoadingTasksProcessor loadingTasksProcessor, AuthenticationModel authenticationModel)
        {
            _sceneLoader = sceneLoader;
            _assetProvider = assetProvider;
            _loadingTasksProcessor = loadingTasksProcessor;
            _authenticationModel = authenticationModel;
        }

        public void Initialize()
        {
            _authenticationModel.AuthenticationCompleted += OnAuthenticationCompleted;
            _authenticationModel.AuthenticationFailed += OnAuthenticationFailed;

            if (_loadingTasksProcessor.IsFinished)
            {
                OnLoadingTasksFinished();
            }
            else
            {
                _loadingTasksProcessor.OnTasksFinished += OnLoadingTasksFinished;
            }
        }

        public void Dispose()
        {
            _loadingTasksProcessor.OnTasksFinished -= OnLoadingTasksFinished;
            _authenticationModel.AuthenticationCompleted -= OnAuthenticationCompleted;
            _authenticationModel.AuthenticationFailed -= OnAuthenticationFailed;

            if (_authView)
            {
                _authView.SignInClicked -= OnSignInClicked;
            }
        }

        private void OnLoadingTasksFinished()
        {
            _loadingTasksProcessor.OnTasksFinished -= OnLoadingTasksFinished;

            if (!_assetProvider.TryGetLoadedComponent(out _authView) || !_authView)
            {
                Debug.LogError("[AuthenticationPresenter] AuthView not provided by SceneAssetProvider.");
                return;
            }

            _authView.SignInClicked += OnSignInClicked;
        }

        private void OnSignInClicked()
        {
            _authenticationModel.StartAuthentication();
        }

        private void OnAuthenticationFailed()
        {
            _authView.RestoreSignInButton();
        }

        private void OnAuthenticationCompleted()
        {
            if (_navigationStarted)
            {
                return;
            }

            _authView.RestoreSignInButton();
            _navigationStarted = true;
            UniTask.Void(GoToMenuAsync);
        }

        private async UniTaskVoid GoToMenuAsync()
        {
            await _sceneLoader.LoadSceneAsync(Scenes.Menu);
        }
    }
}
