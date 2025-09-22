using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;

namespace _Project.Runtime.Asteroid
{
    public class AsteroidsPresenter : IDisposable
    {
        private readonly AsteroidsModel _asteroidsModel;
        private readonly GameModel _gameModel;
        private readonly AsteroidView.Pool _pool;

        private readonly Dictionary<uint, AsteroidView> _activeAsteroids;

        public AsteroidsPresenter(AsteroidsModel asteroidsModel, GameModel gameModel, AsteroidView.Pool pool)
        {
            _asteroidsModel = asteroidsModel;
            _gameModel = gameModel;
            _pool = pool;

            _activeAsteroids = new Dictionary<uint, AsteroidView>();

            _asteroidsModel.AsteroidSpawnRequested += OnSpawnCommand;
            _asteroidsModel.AsteroidDespawnRequested += OnDespawnCommand;
            
            _gameModel.GameStateChanged += OnGameStateChanged;
        }

        public void Dispose()
        {
            _asteroidsModel.AsteroidSpawnRequested -= OnSpawnCommand;
            _asteroidsModel.AsteroidDespawnRequested -= OnDespawnCommand;
            
            _gameModel.GameStateChanged -= OnGameStateChanged;
        }

        private void OnSpawnCommand(AsteroidSpawnCommand command)
        {
            var asteroid = _pool.Spawn(command);
            
            if (!RegisterAsteroid(asteroid))
            {
                _pool.Despawn(asteroid);
                throw new Exception("Failed to register asteroid");
            }
        }

        private void OnDespawnCommand(AsteroidDespawnCommand command)
        {
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