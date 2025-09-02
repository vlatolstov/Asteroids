using System;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Ship
{
    //TODO remove ship instantiation
    public class ShipPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private readonly ShipView.Pool _pool;
        private ShipView _playerView;
        private IMotorInput _motor;

        public ShipPresenter(IModel model, IViewsContainer viewsContainer) : base(model,
            viewsContainer)
        { }

        public void Initialize()
        {
            foreach (var shipView in ViewsContainer.GetViews<ShipView>())
            {
                TryAttachPlayer(shipView);
            }

            ViewsContainer.ViewAdded += OnViewAdded;
            ViewsContainer.ViewRemoved += OnViewRemoved;
            Model.Subscribe<TurnInput>(OnControlChanged);
            Model.Subscribe<ThrustInput>(OnControlChanged);
        }

        public void Dispose()
        {
            ViewsContainer.ViewAdded -= OnViewAdded;
            ViewsContainer.ViewRemoved -= OnViewRemoved;
            Model.Unsubscribe<TurnInput>(OnControlChanged);
            Model.Unsubscribe<ThrustInput>(OnControlChanged);

            DetachPlayer();
        }

        private void TryAttachPlayer(ShipView shipView)
        {
            if (!shipView.IsPlayer) return;

            _playerView = shipView;
            _playerView.Emitted += OnViewEmitted;

            if (shipView.TryGetComponent<IMotorInput>(out var motor))
            {
                _motor = motor;
            }
            else
            {
                Debug.LogWarning($"Motor not found on {shipView.gameObject.name}.");
            }
        }

        private void DetachPlayer()
        {
            if (_playerView)
            {
                _playerView.Emitted -= OnViewEmitted;
            }

            _playerView = null;
            _motor = null;
        }

        private void OnViewAdded(BaseView view)
        {
            if (view is ShipView sv)
            {
                TryAttachPlayer(sv);
            }
        }

        private void OnViewRemoved(BaseView view)
        {
            if (view is ShipView sv && sv == _playerView)
            {
                DetachPlayer();
            }
        }

        private void OnViewEmitted(IData data)
        {
            Model.ChangeData(data);
        }

        private void OnControlChanged()
        {
            if (Model.TryGet(out TurnInput turn) && Model.TryGet(out ThrustInput thrust))
                _motor.SetControls(thrust.Value, turn.Value);
        }

        // private void OnShipSpawnRequest()
        // {
        //     if (Model.TryGet(out ShipSpawned turn) && Model.TryGet(out ThrustInput thrust))
        //         _motor.SetControls(thrust.Value, turn.Value);
        // }
        
        // private void OnShipDespawnRequest()
        // {
        //     if (Model.TryGet(out TurnInput turn) && Model.TryGet(out ThrustInput thrust))
        //         _motor.SetControls(thrust.Value, turn.Value);
        // }
    }
}