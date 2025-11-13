using System;
using _Project.Runtime.Data;
using _Project.Runtime.Constants;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Ship;
using _Project.Runtime.Services;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class BackgroundPresenter : IInitializable, IDisposable
    {
        private readonly ShipModel _shipModel;
        private readonly ViewsContainer _viewsContainer;
        private readonly GameLoadingTaskService _gameLoadingTaskService;
        private readonly IConfigsService _configsService;

        private BackgroundView _bg;
        private bool _initialized;

        public BackgroundPresenter(ShipModel shipModel, ViewsContainer viewsContainer,
            GameLoadingTaskService gameLoadingTaskService, IConfigsService configsService)
        {
            _shipModel = shipModel;
            _viewsContainer = viewsContainer;
            _gameLoadingTaskService = gameLoadingTaskService;
            _configsService = configsService;
        }

        public void Initialize()
        {
            _gameLoadingTaskService.OnTasksFinished += OnLoadingTaskFinished;
        }

        public void Dispose()
        {
            _gameLoadingTaskService.OnTasksFinished -= OnLoadingTaskFinished;

            if (_initialized)
            {
                _shipModel.ShipPoseChanged -= OnShipPoseChanged;
            }
        }

        private void OnLoadingTaskFinished()
        {
            _gameLoadingTaskService.OnTasksFinished -= OnLoadingTaskFinished;
            Setup();
        }

        private void Setup()
        {
            if (_initialized)
            {
                return;
            }

            _bg = _viewsContainer.GetView<BackgroundView>();
            if (_bg == null)
            {
                Debug.LogError("BackgroundView not provided");
                return;
            }

            var config = _configsService.Get<Settings.BackgroundJitterConfig>(AddressablesConfigPaths.General.BackgroundJitter);
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
