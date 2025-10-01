using System;
using _Project.Runtime.Constants;
using UnityEngine;

namespace _Project.Runtime.Score
{
    public class BestScoreService
    {
        private BestScoreData _bestScoreData;

        public int Value => _bestScoreData?.Value ?? 0;

        public BestScoreService()
        {
            if (!TryGetBestScore())
            {
                _bestScoreData = new BestScoreData();
            }
        }

        public void SetBestScore(int score)
        {
            if (_bestScoreData == null)
            {
                _bestScoreData = new BestScoreData();
            }

            if (score <= _bestScoreData.Value)
            {
                return;
            }

            _bestScoreData.Value = score;

            string updatedData = JsonUtility.ToJson(_bestScoreData);
            PlayerPrefs.SetString(DataKeys.BEST_SCORE_KEY, updatedData);
            PlayerPrefs.Save();
        }

        private bool TryGetBestScore()
        {
            string bestScoreDataJson = PlayerPrefs.GetString(DataKeys.BEST_SCORE_KEY);

            if (string.IsNullOrEmpty(bestScoreDataJson))
            {
                return false;
            }

            try
            {
                _bestScoreData = JsonUtility.FromJson<BestScoreData>(bestScoreDataJson);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Failed to parse best score data: {exception.Message}");
                _bestScoreData = null;
                return false;
            }

            if (_bestScoreData == null)
            {
                Debug.LogWarning("Best score data is invalid or empty.");
                return false;
            }

            return true;
        }
    }

    [Serializable]
    public class BestScoreData
    {
        public int Value;
    }
}
