using System;

namespace _Project.Runtime.Models
{
    public class ScoreModel
    {
        private int _totalScore;
        
        public event Action<int> TotalScoreChanged;

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _totalScore += amount;
            TotalScoreChanged?.Invoke(_totalScore);
        }

        public void ChangeTotalScore(int newScore)
        {
            _totalScore = newScore;
            TotalScoreChanged?.Invoke(_totalScore);
        }
    }
}