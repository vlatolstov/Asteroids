using System;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Data;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Ship;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class BackgroundPresenter : IInitializable, IDisposable
    {
        private readonly ShipModel _shipModel;
        private readonly GameLoadingTasksProcessor _gameLoadingTasksProcessor;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly SceneAssetProvider _assetProvider;

        private BackgroundView _bg;
        private bool _initialized;

        public BackgroundPresenter(ShipModel shipModel,
            GameLoadingTasksProcessor gameLoadingTasksProcessor,
            IRemoteConfigProvider remoteConfigProvider,
            SceneAssetProvider assetProvider)
        {
            _shipModel = shipModel;
            _gameLoadingTasksProcessor = gameLoadingTasksProcessor;
            _remoteConfigProvider = remoteConfigProvider;
            _assetProvider = assetProvider;
        }

        public void Initialize()
        {
            if (_gameLoadingTasksProcessor.IsFinished)
            {
                OnLoadingTaskFinished();
            }
            else
            {
                _gameLoadingTasksProcessor.OnTasksFinished += OnLoadingTaskFinished;
            }
        }

        public void Dispose()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;

            if (_initialized)
            {
                _shipModel.ShipPoseChanged -= OnShipPoseChanged;
            }
        }

        private void OnLoadingTaskFinished()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;
            Setup();
        }

        private void Setup()
        {
            if (_initialized)
            {
                return;
            }

            if (!_assetProvider.TryGetLoadedComponent(out _bg))
            {
                Debug.LogError("BackgroundView not provided");
                return;
            }

            if (!_remoteConfigProvider.TryGet(Constants.Config.Background.Jitter, out BackgroundJitterData config))
            {
                Debug.LogWarning("[RemoteConfig] Missing background jitter data.");
                config = new BackgroundJitterData();
            }
            _bg.Initialize(config);

            _shipModel.ShipPoseChanged += OnShipPoseChanged;
            _initialized = true;
        }

        private void OnShipPoseChanged(ShipPose pose)
        {
            _bg.SetPlayerVelocity(pose.Velocity);
        }
    }
}
