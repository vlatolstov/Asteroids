using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "AsteroidsSpawnConfig", menuName = "Settings/AsteroidsSpawnConfig")]
    public class AsteroidsSpawnConfig : ScriptableObject, IAsteroidsSpawnConfig
    {
        [Header("Large Asteroids")]
        [SerializeField]
        private float _interval = 2.5f;

        [SerializeField]
        private float _largeScale = 1f;
        [SerializeField]
        private float _edgeOffset = 2f;

        [SerializeField]
        private float _entrySpeedMin = 2f;

        [SerializeField]
        private float _entrySpeedMax = 5f;

        [SerializeField]
        private float _entryAngleJitterDeg = 20f;

        [Header("Small Asteroids")]
        [SerializeField]
        private float _smallScale = 0.5f;
        [SerializeField]
        private int _smallSplitMin = 2;

        [SerializeField]
        private int _smallSplitMax = 4;

        [SerializeField]
        private float _smallSpeedMin = 3f;

        [SerializeField]
        private float _smallSpeedMax = 6f;

        public float Interval => _interval;
        public float LargeScale => _largeScale;
        public float EdgeOffset => _edgeOffset;
        public float EntrySpeedMin => _entrySpeedMin;
        public float EntrySpeedMax => _entrySpeedMax;
        public float EntryAngleJitterDeg => _entryAngleJitterDeg;
        public float SmallScale => _smallScale;
        public int SmallSplitMin => _smallSplitMin;
        public int SmallSplitMax => _smallSplitMax;
        public float SmallSpeedMin => _smallSpeedMin;
        public float SmallSpeedMax => _smallSpeedMax;
    }
}