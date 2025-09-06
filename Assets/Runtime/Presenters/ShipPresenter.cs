using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Presenters
{
    public class ShipPresenter : BasePresenter<ShipModel>
    {
        private readonly InputModel _inputModel;
        private readonly ShipView.Pool _pool;
        private ShipView _activeShip;

        private IWorldConfig _world;

        public ShipPresenter(ShipModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipView.Pool pool,
            InputModel inputModel, IWorldConfig world) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
            _inputModel = inputModel;
            _world = world;
        }

        public override void Initialize()
        {
            ForwardOn<ShipPose>();
            ForwardOn<ShipSpawnRequest>(publish: true);
            ForwardOn<ShipDespawnRequest>(publish: true);
            ForwardOn<ShipSpawned>(publish: true);
            ForwardOn<ShipDestroyed>(publish: true);

            AddUnsub(_inputModel.Subscribe<FireGunPressed>(OnGunAttackSignal));
            AddUnsub(_inputModel.Subscribe<AoeWeaponAttackPressed>(OnAoeWeaponAttackSignal));
            AddUnsub(_inputModel.Subscribe<ThrustInput>(OnThrustChanged));
            AddUnsub(_inputModel.Subscribe<TurnInput>(OnTurnAxisChanged));
            
            AddUnsub(Model.Subscribe<ShipSpawnCommand>(OnShipSpawnCommand));
            AddUnsub(Model.Subscribe<ShipDespawnCommand>(OnShipDespawnCommand));
        }

        public override void Dispose()
        {
            DetachShip();
        }

        private void TryAttachShip(ShipView shipView)
        {
            _activeShip = shipView;
        }

        private void DetachShip()
        {
            _activeShip = null;
        }

        private void OnThrustChanged()
        {
            if (_inputModel.TryGet(out ThrustInput thrust) &&
                _activeShip)
            {
                _activeShip.SetupMainEngine(thrust.Value != 0);
                _activeShip.Motor.SetThrust(thrust.Value);
            }
        }

        private void OnTurnAxisChanged()
        {
            if (!_inputModel.TryGet(out TurnInput turn) ||
                !_activeShip)
            {
                return;
            }

            switch (turn.Value)
            {
                case > 0:
                    _activeShip.SetupSideEngines(false, true);
                    break;
                case < 0:
                    _activeShip.SetupSideEngines(true, false);
                    break;
                default:
                    _activeShip.SetupSideEngines(false, false);
                    break;
            }

            _activeShip.Motor.SetTurnAxis(turn.Value);
        }

        private void OnGunAttackSignal()
        {
            if (_inputModel.TryGet(out FireGunPressed fire)
                && _activeShip)
            {
                _activeShip.Gun.TryAttack();
            }
        }

        private void OnAoeWeaponAttackSignal()
        {
            if (_inputModel.TryGet(out AoeWeaponAttackPressed fire)
                && _activeShip)
            {
                _activeShip.AoeWeapon.TryAttack();
            }
        }

        private void OnShipSpawnCommand()
        {
            if (_activeShip)
            {
                return;
            }

            if (Model.TryGet(out ShipSpawnCommand spawn))
            {
                var ship = _pool.Spawn(spawn.Position);
                TryAttachShip(ship);
            }
        }

        private void OnShipDespawnCommand()
        {
            if (Model.TryGet(out ShipDespawnCommand despawn))
            {
                var view = ViewsContainer.GetViewById(despawn.ViewId);
                if (!view || view is not ShipView ship)
                {
                    return;
                }

                if (_activeShip == ship)
                {
                    _pool.Despawn(_activeShip);
                    DetachShip();
                }
                else
                {
                    _pool.Despawn(ship);
                }
            }
        }
    }
}