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
        private GameState _previousGameState;
        private bool _ready;

        public ScorePresenter(GameModel gameModel, AsteroidsModel asteroidsModel,
            UfoModel ufoModel, ScoreModel scoreModel, IConfigsService configsService)
        {
            _gameModel = gameModel;
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _scoreModel = scoreModel;
            _configsService = configsService;

            _previousGameState = _gameModel.CurrentState;
        }
        
        public void Initialize()
        {
            UniTask.Void(SetupAsync);
        }

        private async UniTaskVoid SetupAsync()
        {
            await _configsService.LoadAllAsync();
            _scoreConfig = _configsService.Get<ScoreConfig>(AddressablesConfigPaths.General.Score);

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
            if (state == _previousGameState)
            {
                return;
            }

            if (state == GameState.Gameplay &&
                _previousGameState is GameState.Preparing or GameState.Gameplay)
            {
                _scoreModel.ChangeTotalScore(0);
            }
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed destroyed)
        {
            int amount = destroyed.Size switch
            {
                AsteroidSize.Large => _scoreConfig.LargeAsteroidScore,
                AsteroidSize.Small => _scoreConfig.SmallAsteroidScore,
                _ => throw new Exception("Unknown asteroid size")
            };

            _scoreModel.AddScore(amount);
        }

        private void OnUfoDestroyed(UfoDestroyed _)
        {
            _scoreModel.AddScore(_scoreConfig.UfoScore);
        }
    }
}
