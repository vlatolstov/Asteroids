using System;

namespace _Project.Runtime.Score
{
    public class ScoreModel
    {
        private readonly BestScoreService _bestScoreService;

        private int _totalScore;
        public int BestScore => _bestScoreService.Value;

        public event Action<int> TotalScoreChanged;
        public event Action<int> BestScoreChanged;

        public ScoreModel(BestScoreService bestScoreService)
        {
            _bestScoreService = bestScoreService;
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

        public void ChangeTotalScore(int newScore)
        {
            _totalScore = newScore;
            TotalScoreChanged?.Invoke(_totalScore);
            TryUpdateBestScore(_totalScore);
        }

        private void TryUpdateBestScore(int candidate)
        {
            if (candidate <= BestScore)
            {
                return;
            }

            _bestScoreService.SetBestScore(candidate);
            BestScoreChanged?.Invoke(BestScore);
        }
    }
}
