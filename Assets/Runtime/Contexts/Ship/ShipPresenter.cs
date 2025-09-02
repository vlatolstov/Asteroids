using System;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Ship
{
    public class ShipPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private readonly ShipView.Pool _pool;
        private ShipView _player;
        private IMotorInput _motor;

        public ShipPresenter(IModel model, IViewsContainer viewsContainer, ShipView.Pool pool) : base(model,
            viewsContainer)
        {
            _pool = pool;
        }

        public void Initialize()
        {
            Model.Subscribe<TurnInput>(OnControlChanged);
            Model.Subscribe<ThrustInput>(OnControlChanged);
            Model.Subscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Subscribe<ShipDespawnRequest>(OnShipDespawnRequest);
        }

        public void Dispose()
        {
            Model.Unsubscribe<TurnInput>(OnControlChanged);
            Model.Unsubscribe<ThrustInput>(OnControlChanged);
            Model.Unsubscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Unsubscribe<ShipDespawnRequest>(OnShipDespawnRequest);

            DetachPlayer();
        }

        private void TryAttachPlayer(ShipView shipView)
        {
            if (!shipView.IsPlayer) return;

            _player = shipView;
            _player.Emitted += OnEmitted;

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
            if (_player)
            {
                _player.Emitted -= OnEmitted;
            }

            _player = null;
            _motor = null;
        }

        private void OnControlChanged()
        {
            if (Model.TryGet(out TurnInput turn) &&
                Model.TryGet(out ThrustInput thrust) &&
                _player)
            {
                _motor.SetControls(thrust.Value, turn.Value);
            }
        }

        private void OnShipSpawnRequest()
        {
            if (Model.TryGet(out ShipSpawnRequest spawn))
            {
                var player = _pool.Spawn();
                TryAttachPlayer(player);
                player.GetComponent<IMove>()?.SetPose(spawn.Position, Vector2.zero, 0f);
            }
        }

        private void OnShipDespawnRequest()
        {
            if (!_player)
            {
                Debug.LogError("Can't despawn. No player assigned.");
                return;
            }

            if (Model.TryGet<ShipDespawnRequest>(out _))
            {
                DetachPlayer();
                _pool.Despawn(_player);
            }
        }
    }
}