using System;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Ship;
using _Project.Runtime.Views;

namespace _Project.Runtime.Presenters
{
    public class BackgroundPresenter : IDisposable
    {
        private readonly ShipModel _shipModel;
        private readonly BackgroundView _bg;

        public BackgroundPresenter(ShipModel shipModel, ViewsContainer viewsContainer)
        {
            _shipModel = shipModel;
            _bg = viewsContainer.GetView<BackgroundView>();

            _shipModel.ShipPoseChanged += OnShipPoseChanged;
        }

        public void Dispose()
        {
            _shipModel.ShipPoseChanged -= OnShipPoseChanged;
        }


        private void OnShipPoseChanged(ShipPose pose)
        {
            _bg.SetPlayerVelocity(pose.Velocity);
        }
    }
}