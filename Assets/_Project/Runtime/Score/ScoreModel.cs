using System;
using _Project.Runtime.Data;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Services;

namespace _Project.Runtime.Score
{
    public class ScoreModel
    {
        private readonly PlayerDataManager _playerDataManager;

        private int _totalScore;
        private ScoreData _scoreConfig;
        private GameState _previousGameState;
        private bool _hasGameState;
        public bool IsNewRecord { get; private set; }

        public event Action<int> TotalScoreChanged;
        public event Action<bool> NewRecordChanged;

        public ScoreModel(PlayerDataManager playerDataManager)
        {
            _playerDataManager = playerDataManager;
        }

        public void ApplyConfig(ScoreData scoreConfig)
        {
            _scoreConfig = scoreConfig;
        }

        public void SetInitialState(GameState state)
        {
            _previousGameState = state;
            _hasGameState = true;
        }

        public void HandleGameStateChanged(GameState state)
        {
            if (!_hasGameState)
            {
                _previousGameState = state;
                _hasGameState = true;
                return;
            }

            if (state == _previousGameState)
            {
                return;
            }

            if (state == GameState.Gameplay &&
                _previousGameState is GameState.Preparing or GameState.Gameplay)
            {
                ChangeTotalScore(0);
            }

            if (state is GameState.Preparing or GameState.Gameplay)
            {
                SetNewRecord(false);
            }

            _previousGameState = state;
        }

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _totalScore += amount;
            TotalScoreChanged?.Invoke(_totalScore);
            TryUpdateBestScore(_totalScore);
        }

        public void HandleAsteroidDestroyed(AsteroidDestroyed destroyed)
        {
            if (_scoreConfig == null)
            {
                return;
            }

            int amount = destroyed.Size switch
            {
                AsteroidSize.Large => _scoreConfig.LargeAsteroid,
                AsteroidSize.Small => _scoreConfig.SmallAsteroid,
                _ => throw new Exception("Unknown asteroid size")
            };

            AddScore(amount);
        }

        public void HandleUfoDestroyed(UfoDestroyed _)
        {
            if (_scoreConfig == null)
            {
                return;
            }

            AddScore(_scoreConfig.Ufo);
        }

        public void ChangeTotalScore(int newScore)
        {
            _totalScore = newScore;
            TotalScoreChanged?.Invoke(_totalScore);
            TryUpdateBestScore(_totalScore);
        }

        private void TryUpdateBestScore(int candidate)
        {
            if (!_playerDataManager.SetBestScore(candidate))
            {
                return;
            }

            SetNewRecord(true);
        }

        private void SetNewRecord(bool value)
        {
            if (IsNewRecord == value)
            {
                return;
            }

            IsNewRecord = value;
            NewRecordChanged?.Invoke(IsNewRecord);
        }
    }
}
