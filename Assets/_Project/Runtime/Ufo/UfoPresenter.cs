using System;
using System.Collections.Generic;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Movement;
using _Project.Runtime.Services;
using _Project.Runtime.Settings;
using _Project.Runtime.Weapons;
using _Project.Runtime.Ship;
using Zenject;

namespace _Project.Runtime.Ufo
{
    public class UfoPresenter : IInitializable, IDisposable
    {
        private readonly UfoModel _ufoModel;
        private readonly ShipModel _shipModel;
        private readonly CombatModel _combatModel;
        private readonly GameModel _gameModel;
        private readonly IViewPoolsService _poolsService;
        private readonly IConfigsService _configsService;
        private readonly IWorldConfig _worldConfig;

        private readonly Dictionary<uint, UfoView> _activeUfo;
        private UfoView.Pool _pool;
        private bool _subscriptionsActive;

        private ShipPose _targetShip;
        private GameState _gameState;

        private MovementConfig _movementConfig;
        private ProjectileWeaponConfig _gunConfig;
        private ChasingEnemyConfig _chaseConfig;
        private bool _configsReady;

        public UfoPresenter(UfoModel ufoModel, ShipModel shipModel, CombatModel combatModel,
            GameModel gameModel, IViewPoolsService poolsService, IConfigsService configsService,
            IWorldConfig worldConfig)
        {
            _ufoModel = ufoModel;
            _shipModel = shipModel;
            _combatModel = combatModel;
            _gameModel = gameModel;
            _poolsService = poolsService;
            _configsService = configsService;
            _worldConfig = worldConfig;

            _activeUfo = new Dictionary<uint, UfoView>();
        }

        public void Initialize()
        {
            if (_poolsService.IsInitialized)
            {
                OnPoolsInitialized();
            }
            else
            {
                _poolsService.Initialized += OnPoolsInitialized;
            }
        }

        public void Dispose()
        {
            _poolsService.Initialized -= OnPoolsInitialized;

            if (_subscriptionsActive)
            {
                _shipModel.ShipPoseChanged -= OnShipPoseChanged;
                _gameModel.GameStateChanged -= OnGameStateChanged;
                _ufoModel.UfoSpawnRequested -= OnUfoSpawnCommand;
                _ufoModel.UfoDespawnRequested -= OnUfoDespawnCommand;
                _subscriptionsActive = false;
            }
        }

        private void OnPoolsInitialized()
        {
            _poolsService.Initialized -= OnPoolsInitialized;
            _pool = _poolsService.GetPool<UfoView.Pool>();

            if (_subscriptionsActive)
            {
                return;
            }

            _shipModel.ShipPoseChanged += OnShipPoseChanged;
            _gameModel.GameStateChanged += OnGameStateChanged;
            _ufoModel.UfoSpawnRequested += OnUfoSpawnCommand;
            _ufoModel.UfoDespawnRequested += OnUfoDespawnCommand;
            _subscriptionsActive = true;
            _gameState = _gameModel.CurrentState;
            _ufoModel.SetGameState(_gameState);
        }

        private void OnShipPoseChanged(ShipPose shipPose)
        {
            foreach (var ufo in _activeUfo.Values)
            {
                _targetShip = shipPose;
                ufo.UpdateShipPose(shipPose);
            }
        }

        private void OnGameStateChanged(GameState gameState)
        {
            _gameState = gameState;

            foreach (var ufo in _activeUfo.Values)
            {
                ufo.UpdateGameState(gameState);
            }

            _ufoModel.SetGameState(gameState);
        }

        private void OnUfoSpawnCommand(UfoSpawnCommand command)
        {
            if (_pool == null)
            {
                return;
            }

            EnsureConfigs();

            var args = new UfoView.SpawnArgs(
                command,
                new ChasingMotor(_movementConfig, _worldConfig, _chaseConfig),
                _gunConfig,
                _chaseConfig,
                _worldConfig);
            var ufo = _pool.Spawn(args);
            RegisterUfo(ufo);
            _ufoModel.HandleUfoSpawned(new UfoSpawned(ufo.ViewId, ufo.transform.position));
        }

        private void OnUfoDespawnCommand(uint ufoId)
        {
            if (!_activeUfo.TryGetValue(ufoId, out var ufo))
            {
                return;
            }

            UnregisterUfo(ufo);
            _pool?.Despawn(ufo);
        }

        private void RegisterUfo(UfoView ufo)
        {
            _activeUfo.Add(ufo.ViewId, ufo);

            ufo.UpdateShipPose(_targetShip);
            ufo.UpdateGameState(_gameState);

            ufo.ProjectileFired += OnUfoFiredProjectile;
            ufo.Destroyed += OnUfoDestroyed;
            ufo.Offscreen += OnUfoOffscreen;
        }

        private void UnregisterUfo(UfoView ufo)
        {
            ufo.ProjectileFired -= OnUfoFiredProjectile;
            ufo.Destroyed -= OnUfoDestroyed;
            ufo.Offscreen -= OnUfoOffscreen;
            _activeUfo.Remove(ufo.ViewId);
        }

        private void OnUfoDestroyed(UfoDestroyed destroyed)
        {
            _ufoModel.HandleUfoDestroyed(destroyed);
        }

        private void OnUfoOffscreen(UfoOffscreen offscreen)
        {
            _ufoModel.HandleUfoOffscreen(offscreen.ViewId);
        }

        private void OnUfoFiredProjectile(ProjectileShot shot)
        {
            _combatModel.HandleProjectileShot(shot);
        }

        private void EnsureConfigs()
        {
            if (_configsReady)
            {
                return;
            }

            _movementConfig = _configsService.Get<MovementConfig>(AddressablesConfigPaths.Movement.Ufo);
            _gunConfig = _configsService.Get<ProjectileWeaponConfig>(AddressablesConfigPaths.Weapons.UfoBlaster);
            _chaseConfig = _configsService.Get<ChasingEnemyConfig>(AddressablesConfigPaths.Movement.UfoChasing);
            _configsReady = true;
        }
    }
}
