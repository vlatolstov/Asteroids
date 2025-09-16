using _Project.Runtime.Abstract.Configs;
using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "AsteroidsSpawnConfig", menuName = "Settings/Asteroids Spawn Config")]
    public class AsteroidsSpawnConfig : ScriptableObject, IAsteroidsSpawnConfig
    {
        [Header("Common settings")]
        [SerializeField]
        private Sprite[] _spritesVariations;

        [SerializeField]
        private float _rotationMin;

        [SerializeField]
        private float _rotationMax;

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

        public Sprite Sprite => _spritesVariations[Random.Range(0, _spritesVariations.Length)];
        public float AngleRotationDeg => Random.Range(_rotationMin, _rotationMax);
        public float Interval => _interval;
        public float LargeScale => _largeScale;
        public float EdgeOffset => _edgeOffset;
        public float LargeSpeed => Random.Range(Mathf.Max(0, _entrySpeedMin), Mathf.Max(0, _entrySpeedMax));
        public float EntryAngleJitterDeg => _entryAngleJitterDeg;
        public float SmallScale => _smallScale;
        public int SmallSplit => Random.Range(Mathf.Max(0, _smallSplitMin), Mathf.Max(0, _smallSplitMax));
        public float SmallSpeed => Random.Range(Mathf.Max(0, _smallSpeedMin), Mathf.Max(0, _smallSpeedMax));
    }
}