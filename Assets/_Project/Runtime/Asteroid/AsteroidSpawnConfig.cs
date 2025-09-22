using UnityEngine;

namespace _Project.Runtime.Asteroid
{
    [CreateAssetMenu(fileName = "AsteroidsSpawnConfig", menuName = "Settings/Asteroids Spawn Config")]
    public class AsteroidsSpawnConfig : ScriptableObject
    {
        [Header("Common settings")]
        [SerializeField]
        private Sprite[] _spritesVariations;

        public Sprite Sprite => _spritesVariations[Random.Range(0, _spritesVariations.Length)];

        [SerializeField]
        private float _rotationMin = -5f;

        [SerializeField]
        private float _rotationMax = 5f;

        public float AngleRotationDeg => Random.Range(_rotationMin, _rotationMax);

        [Header("Large Asteroids")]
        [field: SerializeField]
        public float Interval { get; private set; } = 1.6f;

        [field: SerializeField]
        public float LargeScale { get; private set; } = 1f;

        [field: SerializeField]
        public float EdgeOffset { get; private set; } = 3f;

        [SerializeField]
        private float _entrySpeedMin = 2f;

        [SerializeField]
        private float _entrySpeedMax = 5f;

        public float LargeSpeed => Random.Range(Mathf.Max(0, _entrySpeedMin), Mathf.Max(0, _entrySpeedMax));

        [field: SerializeField]
        public float EntryAngleJitterDeg { get; private set; } = 40f;


        [Header("Small Asteroids")]
        [field: SerializeField]
        public float SmallScale { get; private set; } = 0.6f;

        [SerializeField]
        private int _smallSplitMin = 3;

        [SerializeField]
        private int _smallSplitMax = 5;

        public int SmallSplit => Random.Range(Mathf.Max(0, _smallSplitMin), Mathf.Max(0, _smallSplitMax) + 1);

        [SerializeField]
        private float _smallSpeedMin = 4f;

        [SerializeField]
        private float _smallSpeedMax = 7f;

        public float SmallSpeed => Random.Range(Mathf.Max(0, _smallSpeedMin), Mathf.Max(0, _smallSpeedMax));
    }
}