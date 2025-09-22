using System;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Settings;
using _Project.Runtime.Ufo;

namespace _Project.Runtime.Presenters
{
    public class ScorePresenter : IDisposable
    {
        private readonly AsteroidsModel _asteroidsModel;
        private readonly UfoModel _ufoModel;
        private readonly GameModel _gameModel;
        private readonly ScoreModel _scoreModel;

        private readonly ScoreConfig _scoreConfig;

        private GameState _previousGameState;

        public ScorePresenter(GameModel gameModel, AsteroidsModel asteroidsModel,
            UfoModel ufoModel, ScoreModel scoreModel, ScoreConfig scoreConfig)
        {
            _gameModel = gameModel;
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _scoreModel = scoreModel;
            _scoreConfig = scoreConfig;

            _previousGameState = _gameModel.CurrentState;
            
            _asteroidsModel.AsteroidDestroyed += OnAsteroidDestroyed;
            _ufoModel.UfoDestroyed += OnUfoDestroyed;
            _gameModel.GameStateChanged += OnGameStateChanged;
        }

        public void Dispose()
        {
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