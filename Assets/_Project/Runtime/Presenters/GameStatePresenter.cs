using System;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class GameStatePresenter : IInitializable, IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly ShipModel _shipModel;

        public GameStatePresenter(GameModel model, ShipModel shipModel)
        {
            _gameModel = model;
            _shipModel = shipModel;
            
            _shipModel.ShipSpawned += OnShipSpawned;
            _shipModel.ShipDestroyed += OnShipDestroyed;
        }
        
        public void Initialize()
        {
            _gameModel.SetGameState(GameState.Preparing);
        }

        public void Dispose()
        {
            _shipModel.ShipSpawned -= OnShipSpawned;
            _shipModel.ShipDestroyed -= OnShipDestroyed;
        }

        private void OnShipSpawned(ShipSpawned _)
        {
            _gameModel.SetGameState(GameState.Gameplay);
        }

        private void OnShipDestroyed(ShipDestroyed _)
        {
            _gameModel.SetGameState(GameState.GameOver);
        }
    }
}