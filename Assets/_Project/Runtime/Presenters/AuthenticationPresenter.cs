using System;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Services;
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
            _authenticationModel.SaveSelectionRequired += OnSaveSelectionRequired;

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
            _authenticationModel.SaveSelectionRequired -= OnSaveSelectionRequired;

            if (_authView)
            {
                _authView.SignInClicked -= OnSignInClicked;
                _authView.LocalSaveSelected -= OnLocalSaveSelected;
                _authView.CloudSaveSelected -= OnCloudSaveSelected;
            }
        }

        private void OnLoadingTasksFinished()
        {
            _loadingTasksProcessor.OnTasksFinished -= OnLoadingTasksFinished;

            if (!_assetProvider.TryGetLoadedComponent(AddressablesPrefabsPaths.AuthView, out _authView) || !_authView)
            {
                Debug.LogError("[AuthenticationPresenter] AuthView not provided by SceneAssetProvider.");
                return;
            }

            _authView.SignInClicked += OnSignInClicked;
            _authView.LocalSaveSelected += OnLocalSaveSelected;
            _authView.CloudSaveSelected += OnCloudSaveSelected;
            _authView.HideSaveSelectionWindow();
        }

        private void OnSignInClicked()
        {
            _authView.HideSaveSelectionWindow();
            _authenticationModel.StartAuthentication();
        }

        private void OnAuthenticationFailed()
        {
            _authView.HideSaveSelectionWindow();
            _authView.RestoreSignInButton();
        }

        private void OnAuthenticationCompleted()
        {
            if (_navigationStarted)
            {
                return;
            }

            _authView.HideSaveSelectionWindow();
            _authView.RestoreSignInButton();
            _navigationStarted = true;
            UniTask.Void(GoToMenuAsync);
        }

        private void OnSaveSelectionRequired(SaveSelectionInfo selectionInfo)
        {
            _authView.SetSaveSelectionInfo(selectionInfo);
            _authView.ShowSaveSelectionWindow();
        }

        private void OnLocalSaveSelected()
        {
            _authView.HideSaveSelectionWindow();
            _authenticationModel.SelectLocalSave();
        }

        private void OnCloudSaveSelected()
        {
            _authView.HideSaveSelectionWindow();
            _authenticationModel.SelectCloudSave();
        }

        private async UniTaskVoid GoToMenuAsync()
        {
            await _sceneLoader.LoadSceneAsync(Scenes.Menu);
        }
    }
}
