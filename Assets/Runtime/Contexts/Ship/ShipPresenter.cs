using System;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Ship
{
    public class ShipPresenter : BasePresenter<IModel>
    {
        private readonly ShipView.Pool _pool;
        private ShipView _player;
        
        public ShipPresenter(IModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipView.Pool pool) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
        }

        public override void Initialize()
        {
            base.Initialize();
            Model.Subscribe<TurnInput>(OnControlChanged);
            Model.Subscribe<ThrustInput>(OnControlChanged);
            Model.Subscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Subscribe<ShipDespawnRequest>(OnShipDespawnRequest);
        }

        public override void Dispose()
        {
            base.Dispose();
            Model.Unsubscribe<TurnInput>(OnControlChanged);
            Model.Unsubscribe<ThrustInput>(OnControlChanged);
            Model.Unsubscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Unsubscribe<ShipDespawnRequest>(OnShipDespawnRequest);

            DetachPlayer();
        }

        private void TryAttachPlayer(ShipView shipView)
        {
            if (!shipView.IsPlayer)
            {
                return;
            }

            _player = shipView;
        }

        private void DetachPlayer()
        {
            _player = null;
        }

        private void OnControlChanged()
        {
            if (Model.TryGet(out TurnInput turn) &&
                Model.TryGet(out ThrustInput thrust) &&
                _player)
            {
                _player.Motor.SetControls(thrust.Value, turn.Value);
            }
        }

        private void OnShipSpawnRequest()
        {
            if (Model.TryGet(out ShipSpawnRequest spawn))
            {
                var player = _pool.Spawn();
                TryAttachPlayer(player);
                if (_player)
                {
                    _player.Motor.SetPose(spawn.Position, Vector2.zero, 0f);
                }
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