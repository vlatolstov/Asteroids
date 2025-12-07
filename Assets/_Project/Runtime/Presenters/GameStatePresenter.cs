using System;
using _Project.Runtime.Data;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Ship;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class GameStatePresenter : IInitializable, IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly ShipModel _shipModel;
        private readonly GameLoadingTasksProcessor _gameLoadingTasksProcessor;

        public GameStatePresenter(GameModel model, ShipModel shipModel, GameLoadingTasksProcessor gameLoadingTasksProcessor)
        {
            _gameModel = model;
            _shipModel = shipModel;
            _gameLoadingTasksProcessor = gameLoadingTasksProcessor;
        }
        
        public void Initialize()
        {
            _shipModel.ShipSpawned += OnShipSpawned;
            _shipModel.ShipDestroyed += OnShipDestroyed;

            _gameLoadingTasksProcessor.OnTasksFinished += OnLoadingFinished;
        }

        public void Dispose()
        {
            _shipModel.ShipSpawned -= OnShipSpawned;
            _shipModel.ShipDestroyed -= OnShipDestroyed;
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingFinished;
        }

        private void OnLoadingFinished()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingFinished;
            _gameModel.SetGameState(GameState.Gameplay);
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
