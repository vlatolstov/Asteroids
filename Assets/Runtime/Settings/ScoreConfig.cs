using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Settings/ScoreConfig", order = 0)]
    public class ScoreConfig : ScriptableObject, IScoreConfig
    {
        [SerializeField]
        private int _bigAsteroidScore;

        [SerializeField]
        private int _smallAsteroidScore;

        [SerializeField]
        private int _ufoScore;

        public int LargeAsteroidScore => _bigAsteroidScore;
        public int SmallAsteroidScore => _smallAsteroidScore;
        public int UfoScore => _ufoScore;
    }
}