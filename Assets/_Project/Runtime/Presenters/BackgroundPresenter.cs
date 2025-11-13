using System;
using _Project.Runtime.Data;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Ship;
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

        private BackgroundView _bg;
        private bool _initialized;

        public BackgroundPresenter(ShipModel shipModel, ViewsContainer viewsContainer, GameLoadingTaskService gameLoadingTaskService)
        {
            _shipModel = shipModel;
            _viewsContainer = viewsContainer;
            _gameLoadingTaskService = gameLoadingTaskService;
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

            _shipModel.ShipPoseChanged += OnShipPoseChanged;
            _initialized = true;
        }

        private void OnShipPoseChanged(ShipPose pose)
        {
            _bg.SetPlayerVelocity(pose.Velocity);
        }
    }
}
