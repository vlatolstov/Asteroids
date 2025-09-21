using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Settings/Score Config", order = 0)]
    public class ScoreConfig : ScriptableObject
    {
        [field: SerializeField]
        public int LargeAsteroidScore { get; private set; }

        [field: SerializeField]
        public int SmallAsteroidScore { get; private set; }

        [field: SerializeField]
        public int UfoScore { get; private set; }
    }
}