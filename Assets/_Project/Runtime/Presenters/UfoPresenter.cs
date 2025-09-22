using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Views;

namespace _Project.Runtime.Presenters
{
    public class UfoPresenter : IDisposable
    {
        private readonly UfoModel _ufoModel;
        private readonly ShipModel _shipModel;
        private readonly CombatModel _combatModel;
        private readonly GameModel _gameModel;
        private readonly UfoView.Pool _pool;

        private readonly Dictionary<uint, UfoView> _activeUfo;

        private ShipPose _targetShip;
        private GameState _gameState;

        public UfoPresenter(UfoModel ufoModel, ShipModel shipModel, CombatModel combatModel,
            GameModel gameModel, UfoView.Pool pool)
        {
            _ufoModel = ufoModel;
            _shipModel = shipModel;
            _combatModel = combatModel;
            _gameModel = gameModel;
            _pool = pool;

            _activeUfo = new Dictionary<uint, UfoView>();
            
            _shipModel.ShipPoseChanged += OnShipPoseChanged;
            _gameModel.GameStateChanged += OnGameStateChanged;
            _ufoModel.UfoSpawnRequested += OnUfoSpawnCommand;
            _ufoModel.UfoDespawnRequested += OnUfoDespawnCommand;
        }
        
        public void Dispose()
        {
            _shipModel.ShipPoseChanged -= OnShipPoseChanged;
            _gameModel.GameStateChanged -= OnGameStateChanged;
            _ufoModel.UfoSpawnRequested -= OnUfoSpawnCommand;
            _ufoModel.UfoDespawnRequested -= OnUfoDespawnCommand;
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
            var ufo = _pool.Spawn(command);
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
            _pool.Despawn(ufo);
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
    }
}