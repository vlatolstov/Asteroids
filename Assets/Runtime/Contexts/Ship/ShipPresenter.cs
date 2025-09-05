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
        private ShipView _ship;

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

            _inputModel.Subscribe<FireGunPressed>(OnGunAttackSignal);
            _inputModel.Subscribe<ThrustInput>(OnThrustChanged);
            _inputModel.Subscribe<TurnInput>(OnTurnAxisChanged);
            Model.Subscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Subscribe<ShipDespawnRequest>(OnShipDespawnRequest);
        }

        public override void Dispose()
        {
            base.Dispose();

            _inputModel.Unsubscribe<FireGunPressed>(OnGunAttackSignal);
            _inputModel.Unsubscribe<ThrustInput>(OnThrustChanged);
            _inputModel.Unsubscribe<TurnInput>(OnTurnAxisChanged);
            Model.Unsubscribe<ShipSpawnRequest>(OnShipSpawnRequest);
            Model.Unsubscribe<ShipDespawnRequest>(OnShipDespawnRequest);

            DetachShip();
        }

        private void TryAttachShip(ShipView shipView)
        {
            _ship = shipView;
        }

        private void DetachShip()
        {
            _ship = null;
        }

        private void OnThrustChanged()
        {
            if (_inputModel.TryGet(out ThrustInput thrust) &&
                _ship)
            {
                _ship.SetupMainEngine(thrust.Value != 0);
                _ship.Motor.SetThrust(thrust.Value);
            }
        }

        private void OnTurnAxisChanged()
        {
            if (!_inputModel.TryGet(out TurnInput turn) ||
                !_ship)
            {
                return;
            }

            switch (turn.Value)
            {
                case > 0:
                    _ship.SetupSideEngines(true, false);
                    break;
                case < 0:
                    _ship.SetupSideEngines(false, true);
                    break;
                default:
                    _ship.SetupSideEngines(false, false);
                    break;
            }

            _ship.Motor.SetTurnAxis(turn.Value);
        }

        private void OnGunAttackSignal()
        {
            if (_inputModel.TryGet(out FireGunPressed fire))
            {
                _ship.Gun.TryAttack();
            }
        }

        private void OnShipSpawnRequest()
        {
            if (Model.TryGet(out ShipSpawnRequest spawn))
            {
                var ship = _pool.Spawn();
                TryAttachShip(ship);
                if (_ship)
                {
                    _ship.Motor.SetPose(spawn.Position, Vector2.zero, 0f);
                }
            }
        }

        private void OnShipDespawnRequest()
        {
            if (!_ship)
            {
                Debug.LogError("Can't despawn. No player assigned.");
                return;
            }

            if (Model.TryGet<ShipDespawnRequest>(out _))
            {
                DetachShip();
                _pool.Despawn(_ship);
            }
        }
    }
}