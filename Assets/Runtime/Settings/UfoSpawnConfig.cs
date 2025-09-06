using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "UfoSpawnConfig", menuName = "Settings/UFO Spawn Config")]
    public sealed class UfoSpawnConfig : ScriptableObject, IUfoSpawnConfig
    {
        [Header("Visual")]
        [SerializeField]
        private Sprite[] _sprites;

        [SerializeField, Min(0f)]
        private float _scale = 1f;


        [Header("Timing / Population")]
        [SerializeField, Min(0f), Tooltip("Delay before the very first spawn (seconds).")]
        private float _initialDelay = 2.0f;

        [SerializeField, Min(0.01f)]
        private float _interval = 6.0f;

        [SerializeField, Min(0)]
        private int _maxAlive = 1;


        [Header("Entry Placement & Motion")]
        [SerializeField, Min(0f)]
        private float _edgeOffset = 2.0f;

        [SerializeField, Min(0f)]
        private float _speed = 6.0f;

        [SerializeField, Min(0f)]
        private float _entryAngleJitterDeg = 8f;


        public Sprite Sprite => _sprites[Random.Range(0, _sprites.Length)];
        public float Scale => _scale;
        public float InitialDelay => _initialDelay;
        public float Interval => _interval;
        public int MaxAlive => _maxAlive;
        public float EdgeOffset => _edgeOffset;
        public float Speed => _speed;
        public float EntryAngleJitterDeg => _entryAngleJitterDeg;

        private void OnValidate()
        {
            _scale = Mathf.Max(0f, _scale);
            _initialDelay = Mathf.Max(0f, _initialDelay);
            _interval = Mathf.Max(0.01f, _interval);
            _maxAlive = Mathf.Max(0, _maxAlive);
            _edgeOffset = Mathf.Max(0f, _edgeOffset);
            _speed = Mathf.Max(0f, _speed);
            _entryAngleJitterDeg = Mathf.Max(0f, _entryAngleJitterDeg);
        }
    }
}