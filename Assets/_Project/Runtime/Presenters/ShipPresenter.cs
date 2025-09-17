using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Views;
using Zenject;

namespace _Project.Runtime.Presenters
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

            _inputModel.FireGunPressed += OnGunAttackSignal;
            _inputModel.AoeAttackPressed += OnAoeWeaponAttackSignal;
            _inputModel.ThrustChanged += OnThrustChanged;
            _inputModel.TurnChanged += OnTurnAxisChanged;

            AddUnsub(Model.Subscribe<ShipSpawnCommand>(OnShipSpawnCommand));
            AddUnsub(Model.Subscribe<ShipDespawnCommand>(OnShipDespawnCommand));
        }

        public override void Dispose()
        {
            _inputModel.FireGunPressed -= OnGunAttackSignal;
            _inputModel.AoeAttackPressed -= OnAoeWeaponAttackSignal;
            _inputModel.ThrustChanged -= OnThrustChanged;
            _inputModel.TurnChanged -= OnTurnAxisChanged;
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

        private void OnThrustChanged(float value)
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.SetupMainEngine(value != 0);
            _activeShip.Motor.SetThrust(value);
        }

        private void OnTurnAxisChanged(float value)
        {
            if (!_activeShip)
            {
                return;
            }

            switch (value)
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

            _activeShip.Motor.SetTurnAxis(value);
        }

        private void OnGunAttackSignal()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.Gun.TryAttack();
        }

        private void OnAoeWeaponAttackSignal()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.AoeWeapon.TryAttack();
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