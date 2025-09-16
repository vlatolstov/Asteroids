using _Project.Runtime.Abstract.Configs;
using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Settings/Score Config", order = 0)]
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