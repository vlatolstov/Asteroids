using System;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Services;
using _Project.Runtime.Settings;
using _Project.Runtime.Ufo;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Runtime.Score
{
    public class ScorePresenter : IInitializable, IDisposable
    {
        private readonly AsteroidsModel _asteroidsModel;
        private readonly UfoModel _ufoModel;
        private readonly GameModel _gameModel;
        private readonly ScoreModel _scoreModel;
        private readonly IConfigsService _configsService;

        private ScoreConfig _scoreConfig;
        private bool _ready;

        public ScorePresenter(GameModel gameModel, AsteroidsModel asteroidsModel,
            UfoModel ufoModel, ScoreModel scoreModel, IConfigsService configsService)
        {
            _gameModel = gameModel;
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _scoreModel = scoreModel;
            _configsService = configsService;
        }
        
        public void Initialize()
        {
            UniTask.Void(SetupAsync);
        }

        private async UniTaskVoid SetupAsync()
        {
            await _configsService.LoadAllAsync();
            _scoreConfig = _configsService.Get<ScoreConfig>(AddressablesConfigPaths.General.Score);

            _scoreModel.ApplyConfig(_scoreConfig);
            _scoreModel.SetInitialState(_gameModel.CurrentState);

            _asteroidsModel.AsteroidDestroyed += OnAsteroidDestroyed;
            _ufoModel.UfoDestroyed += OnUfoDestroyed;
            _gameModel.GameStateChanged += OnGameStateChanged;
            _ready = true;
        }

        public void Dispose()
        {
            if (!_ready)
            {
                return;
            }

            _asteroidsModel.AsteroidDestroyed -= OnAsteroidDestroyed;
            _ufoModel.UfoDestroyed -= OnUfoDestroyed;
            _gameModel.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            _scoreModel.HandleGameStateChanged(state);
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed destroyed)
        {
            _scoreModel.HandleAsteroidDestroyed(destroyed);
        }

        private void OnUfoDestroyed(UfoDestroyed _)
        {
            _scoreModel.HandleUfoDestroyed(_);
        }
    }
}
