using System;
using System.Collections.Generic;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Movement;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Services;
using Zenject;

namespace _Project.Runtime.Asteroid
{
    public class AsteroidsPresenter : IInitializable, IDisposable
    {
        private readonly AsteroidsModel _asteroidsModel;
        private readonly GameModel _gameModel;
        private readonly IViewPoolsService _poolsService;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private readonly IWorldConfig _worldConfig;

        private readonly Dictionary<uint, AsteroidView> _activeAsteroids;
        private AsteroidView.Pool _pool;
        private bool _subscriptionsActive;
        private MovementConfigData _movementConfig;
        private bool _configsReady;
        private bool _initialized;

        public AsteroidsPresenter(AsteroidsModel asteroidsModel, GameModel gameModel,
            IViewPoolsService poolsService, IRemoteConfigProvider remoteConfigProvider, IWorldConfig worldConfig)
        {
            _asteroidsModel = asteroidsModel;
            _gameModel = gameModel;
            _poolsService = poolsService;
            _remoteConfigProvider = remoteConfigProvider;
            _worldConfig = worldConfig;

            _activeAsteroids = new Dictionary<uint, AsteroidView>();
        }

        public void Initialize()
        {
            if (_poolsService.IsReady)
            {
                OnPoolsReady();
                return;
            }

            _poolsService.Ready += OnPoolsReady;
        }

        public void Dispose()
        {
            _poolsService.Ready -= OnPoolsReady;

            if (_subscriptionsActive)
            {
                _asteroidsModel.AsteroidSpawnRequested -= OnSpawnCommand;
                _asteroidsModel.AsteroidDespawnRequested -= OnDespawnCommand;

                _gameModel.GameStateChanged -= OnGameStateChanged;
                _subscriptionsActive = false;
            }
        }

        private void OnSpawnCommand(AsteroidSpawnCommand command)
        {
            if (_pool == null)
            {
                return;
            }

            EnsureConfigs();

            var args = new AsteroidView.SpawnArgs(command, new InertialMotor(_movementConfig, _worldConfig));
            var asteroid = _pool.Spawn(args);

            if (!RegisterAsteroid(asteroid))
            {
                _pool.Despawn(asteroid);
                throw new Exception("Failed to register asteroid");
            }
        }

        private void OnDespawnCommand(AsteroidDespawnCommand command)
        {
            if (_pool == null)
            {
                return;
            }

            if (!_activeAsteroids.TryGetValue(command.ViewId, out var asteroid) ||
                !UnregisterAsteroid(asteroid))
            {
                throw new Exception("Asteroid has not been registered");
            }

            _pool.Despawn(asteroid);
        }

        private void OnGameStateChanged(GameState state)
        {
            _asteroidsModel.SetGameState(state);
        }

        private void EnsureConfigs()
        {
            if (_configsReady)
            {
                return;
            }

            if (!_remoteConfigProvider.TryGet(Config.Asteroids.Movement, out _movementConfig))
            {
                _movementConfig = new MovementConfigData();
            }
            _configsReady = true;
        }

        private bool RegisterAsteroid(AsteroidView asteroid)
        {
            if (!_activeAsteroids.TryAdd(asteroid.ViewId, asteroid))
            {
                return false;
            }

            asteroid.Destroyed += OnAsteroidDestroyed;
            asteroid.Offscreen += OnAsteroidOffscreen;
            return true;
        }

        private bool UnregisterAsteroid(AsteroidView asteroid)
        {
            if (!_activeAsteroids.Remove(asteroid.ViewId))
            {
                return false;
            }

            asteroid.Destroyed -= OnAsteroidDestroyed;
            asteroid.Offscreen -= OnAsteroidOffscreen;
            return true;
        }

        private void OnAsteroidOffscreen(AsteroidOffscreen offscreen)
        {
            _asteroidsModel.HandleAsteroidOffscreen(offscreen);
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed destroyed)
        {
            _asteroidsModel.HandleAsteroidDestroyed(destroyed);
        }

        private void OnPoolsReady()
        {
            if (_initialized)
            {
                return;
            }

            _poolsService.Ready -= OnPoolsReady;
            _pool = _poolsService.GetPool<AsteroidView.Pool>();

            if (_subscriptionsActive)
            {
                _initialized = true;
                return;
            }

            _asteroidsModel.AsteroidSpawnRequested += OnSpawnCommand;
            _asteroidsModel.AsteroidDespawnRequested += OnDespawnCommand;
            _gameModel.GameStateChanged += OnGameStateChanged;
            _subscriptionsActive = true;
            _asteroidsModel.SetGameState(_gameModel.CurrentState);
            _initialized = true;
        }
    }
}
