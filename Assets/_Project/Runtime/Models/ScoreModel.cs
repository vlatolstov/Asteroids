using System;

namespace _Project.Runtime.Models
{
    public class ScoreModel
    {
        public event Action<int> TotalScoreChanged;

        public int TotalScore { get; private set; }

        public ScoreModel()
        {
            TotalScore = 0;
        }

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            TotalScore += amount;
            TotalScoreChanged?.Invoke(TotalScore);
        }

        public void ChangeTotalScore(int newScore)
        {
            TotalScore = newScore;
            TotalScoreChanged?.Invoke(TotalScore);
        }
    }
}