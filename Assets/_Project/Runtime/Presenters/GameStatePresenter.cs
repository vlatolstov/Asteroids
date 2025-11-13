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
        private readonly GameLoadingTaskService _gameLoadingTaskService;

        public GameStatePresenter(GameModel model, ShipModel shipModel, GameLoadingTaskService gameLoadingTaskService)
        {
            _gameModel = model;
            _shipModel = shipModel;
            _gameLoadingTaskService = gameLoadingTaskService;
        }
        
        public void Initialize()
        {
            _shipModel.ShipSpawned += OnShipSpawned;
            _shipModel.ShipDestroyed += OnShipDestroyed;

            _gameLoadingTaskService.OnTasksFinished += OnLoadingFinished;
        }

        public void Dispose()
        {
            _shipModel.ShipSpawned -= OnShipSpawned;
            _shipModel.ShipDestroyed -= OnShipDestroyed;
            _gameLoadingTaskService.OnTasksFinished -= OnLoadingFinished;
        }

        private void OnLoadingFinished()
        {
            _gameLoadingTaskService.OnTasksFinished -= OnLoadingFinished;
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
