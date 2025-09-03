using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Ship
{
    public class ShipPresenter : BasePresenter<ShipModel>
    {
        private readonly InputModel _inputModel;
        private readonly ShipView.Pool _pool;
        private ShipView _player;

        public ShipPresenter(ShipModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipView.Pool pool,
            InputModel inputModel) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
            _inputModel = inputModel;
        }

        public override void Initialize()
        {
            base.Initialize();

            ForwardOn<ShipPose>();
            ForwardOn<ShipSpawnRequest>(publish: true);
            ForwardOn<ShipDespawnRequest>(publish: true);

            _inputModel.Subscribe<ThrustInput>(OnThrustChanged);
            _inputModel.Subscribe<TurnInput>(OnTurnAxisChanged);
            Model.Subscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Subscribe<ShipDespawnRequest>(OnShipDespawnRequest);
        }

        public override void Dispose()
        {
            base.Dispose();

            _inputModel.Unsubscribe<ThrustInput>(OnThrustChanged);
            _inputModel.Unsubscribe<TurnInput>(OnTurnAxisChanged);
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

        private void OnThrustChanged()
        {
            if (_inputModel.TryGet(out ThrustInput thrust) &&
                _player)
            {
                _player.Motor.SetThrust(thrust.Value);
            }
        }

        private void OnTurnAxisChanged()
        {
            if (_inputModel.TryGet(out TurnInput turn) &&
                _player)
            {
                _player.Motor.SetTurnAxis(turn.Value);
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