using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Pooling;
using Zenject;

namespace _Project.Runtime.Asteroid
{
    public class AsteroidsPresenter : IInitializable, IDisposable
    {
        private readonly AsteroidsModel _asteroidsModel;
        private readonly GameModel _gameModel;
        private readonly IViewPoolsService _poolsService;

        private readonly Dictionary<uint, AsteroidView> _activeAsteroids;
        private AsteroidView.Pool _pool;
        private bool _subscriptionsActive;

        public AsteroidsPresenter(AsteroidsModel asteroidsModel, GameModel gameModel,
            IViewPoolsService poolsService)
        {
            _asteroidsModel = asteroidsModel;
            _gameModel = gameModel;
            _poolsService = poolsService;

            _activeAsteroids = new Dictionary<uint, AsteroidView>();
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
                _asteroidsModel.AsteroidSpawnRequested -= OnSpawnCommand;
                _asteroidsModel.AsteroidDespawnRequested -= OnDespawnCommand;

                _gameModel.GameStateChanged -= OnGameStateChanged;
                _subscriptionsActive = false;
            }
        }

        private void OnPoolsInitialized()
        {
            _poolsService.Initialized -= OnPoolsInitialized;
            _pool = _poolsService.GetPool<AsteroidView.Pool>();

            if (_subscriptionsActive)
            {
                return;
            }

            _asteroidsModel.AsteroidSpawnRequested += OnSpawnCommand;
            _asteroidsModel.AsteroidDespawnRequested += OnDespawnCommand;
            _gameModel.GameStateChanged += OnGameStateChanged;
            _subscriptionsActive = true;
            _asteroidsModel.SetGameState(_gameModel.CurrentState);
        }

        private void OnSpawnCommand(AsteroidSpawnCommand command)
        {
            if (_pool == null)
            {
                return;
            }

            var asteroid = _pool.Spawn(command);

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
    }
}
