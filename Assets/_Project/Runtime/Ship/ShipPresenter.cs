using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Movement;
using _Project.Runtime.Services;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Ship
{
    public class ShipPresenter : IInitializable, IDisposable
    {
        private readonly ShipModel _shipModel;
        private readonly GameModel _gameModel;
        private readonly CombatModel _combatModel;
        private readonly InputModel _inputModel;
        private readonly IViewPoolsService _poolsService;
        private readonly IConfigsService _configsService;
        private readonly IWorldConfig _worldConfig;

        private ShipView _activeShip;
        private ShipView.Pool _pool;
        private bool _subscriptionsActive;

        private MovementConfig _movementConfig;
        private ProjectileWeaponConfig _shipGunConfig;
        private AoeWeaponConfig _shipAoeConfig;
        private bool _configsReady;

        public ShipPresenter(ShipModel shipModel, CombatModel combatModel, InputModel inputModel,
            IViewPoolsService poolsService, GameModel gameModel, IConfigsService configsService,
            IWorldConfig worldConfig)
        {
            _shipModel = shipModel;
            _combatModel = combatModel;
            _inputModel = inputModel;
            _poolsService = poolsService;
            _gameModel = gameModel;
            _configsService = configsService;
            _worldConfig = worldConfig;
        }

        public void Initialize()
        {
            _pool = _poolsService.GetPool<ShipView.Pool>();
            Subscribe();
        }

        private void Subscribe()
        {
            if (_subscriptionsActive)
            {
                return;
            }

            _inputModel.FireGunPressed += OnGunAttackSignal;
            _inputModel.AoeAttackPressed += OnAoeWeaponAttackSignal;
            _inputModel.ThrustChanged += OnThrustChanged;
            _inputModel.TurnChanged += OnTurnAxisChanged;
            
            _gameModel.GameStateChanged += OnGameStateChanged;

            _shipModel.ShipSpawnCommandRequested += OnShipSpawnCommand;
            _shipModel.ShipDespawnCommandRequested += OnShipDespawnCommand;
            _subscriptionsActive = true;

            if (_gameModel.CurrentState == GameState.Gameplay)
            {
                _shipModel.RequestSpawn();
            }
        }

        public void Dispose()
        {
            if (_subscriptionsActive)
            {
                _inputModel.FireGunPressed -= OnGunAttackSignal;
                _inputModel.AoeAttackPressed -= OnAoeWeaponAttackSignal;
                _inputModel.ThrustChanged -= OnThrustChanged;
                _inputModel.TurnChanged -= OnTurnAxisChanged;
            
                _gameModel.GameStateChanged -= OnGameStateChanged;

                _shipModel.ShipSpawnCommandRequested -= OnShipSpawnCommand;
                _shipModel.ShipDespawnCommandRequested -= OnShipDespawnCommand;
                _subscriptionsActive = false;
            }

            DetachShip();
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Gameplay)
            {
                _shipModel.RequestSpawn();
            }
        }
        
        private void AttachShip(ShipView shipView)
        {
            if (!shipView)
            {
                return;
            }

            _activeShip = shipView;
            shipView.PoseChanged += OnShipPoseChanged;
            shipView.ProjectileFired += OnProjectileFired;
            shipView.ProjectileWeaponStateChanged += OnProjectileWeaponStateChanged;
            shipView.AoeAttacked += OnAoeAttackReleased;
            shipView.AoeWeaponStateChanged += OnAoeWeaponStateChanged;
            shipView.Destroyed += OnShipDestroyed;
        }

        private void DetachShip()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.PoseChanged -= OnShipPoseChanged;
            _activeShip.ProjectileFired -= OnProjectileFired;
            _activeShip.ProjectileWeaponStateChanged -= OnProjectileWeaponStateChanged;
            _activeShip.AoeAttacked -= OnAoeAttackReleased;
            _activeShip.AoeWeaponStateChanged -= OnAoeWeaponStateChanged;
            _activeShip.Destroyed -= OnShipDestroyed;
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

            _activeShip.TryShootProjectile();
        }

        private void OnAoeWeaponAttackSignal()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.TryReleaseAoeAttack();
        }

        private void OnShipSpawnCommand(Vector3 position)
        {
            if (_activeShip)
            {
                return;
            }

            if (_pool == null)
            {
                return;
            }

            EnsureConfigs();

            var args = new ShipView.SpawnArgs(
                position,
                new PlayerMotor(_movementConfig, _worldConfig),
                _shipGunConfig,
                _shipAoeConfig);
            var ship = _pool.Spawn(args);
            AttachShip(ship);
            _shipModel.HandleShipSpawned(new ShipSpawned(ship.transform.position, ship.transform.localScale));
            _shipModel.UpdatePose(new ShipPose(position, Vector2.zero, 0f));
        }

        private void OnShipDespawnCommand()
        {
            if (!_activeShip || _pool == null)
            {
                return;
            }

            _pool.Despawn(_activeShip);
            DetachShip();
        }

        private void OnShipPoseChanged(ShipPose shipPose)
        {
            _shipModel.UpdatePose(shipPose);
        }

        private void OnProjectileWeaponStateChanged(ProjectileWeaponState state)
        {
            _shipModel.UpdateProjectileWeaponState(state);
        }

        private void OnAoeWeaponStateChanged(AoeWeaponState state)
        {
            _shipModel.UpdateAoeWeaponState(state);
        }

        private void OnShipDestroyed(ShipDestroyed destroyed)
        {
            _shipModel.HandleShipDestroyed(destroyed);
        }

        private void OnProjectileFired(ProjectileShot shot)
        {
            _combatModel.HandleProjectileShot(shot);
        }

        private void OnAoeAttackReleased(AoeAttackReleased attack)
        {
            _combatModel.HandleAoeAttackReleased(attack);
        }

        private void EnsureConfigs()
        {
            if (_configsReady)
            {
                return;
            }

            _movementConfig = _configsService.Get<MovementConfig>(AddressablesConfigPaths.Movement.Ship);
            _shipGunConfig = _configsService.Get<ProjectileWeaponConfig>(AddressablesConfigPaths.Weapons.ShipGun);
            _shipAoeConfig = _configsService.Get<AoeWeaponConfig>(AddressablesConfigPaths.Weapons.ShipLaser);
            _configsReady = true;
        }
    }
}
