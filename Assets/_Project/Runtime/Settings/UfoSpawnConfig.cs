using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "UfoSpawnConfig", menuName = "Settings/UFO Spawn Config")]
    public sealed class UfoSpawnConfig : ScriptableObject
    {
        [Header("Visual")]
        [SerializeField]
        private Sprite[] _sprites;
        public Sprite Sprite => _sprites[Random.Range(0, _sprites.Length)];

        [field:SerializeField, Min(0f)]
        public float Scale { get; private set; }


        [Header("Timing / Population")]
        [field:SerializeField, Min(0f), Tooltip("Delay before the very first spawn (seconds).")]
        public float InitialDelay{ get; private set; }

        [field:SerializeField, Min(0.01f)]
        public float Interval{ get; private set; }

        [field:SerializeField, Min(0)]
        public int MaxAlive { get; private set; }
        
        
        
        
        


        [Header("Entry Placement & Motion")]
        [field:SerializeField, Min(0f)]
        public float EdgeOffset { get; private set; }

        [field:SerializeField, Min(0f)]
        public float Speed { get; private set; }

        [field:SerializeField, Min(0f)]
        public float EntryAngleJitterDeg { get; private set; }
        
        
        
        

        private void OnValidate()
        {
            Scale = Mathf.Max(0f, Scale);
            InitialDelay = Mathf.Max(0f, InitialDelay);
            Interval = Mathf.Max(0.01f, Interval);
            MaxAlive = Mathf.Max(0, MaxAlive);
            EdgeOffset = Mathf.Max(0f, EdgeOffset);
            Speed = Mathf.Max(0f, Speed);
            EntryAngleJitterDeg = Mathf.Max(0f, EntryAngleJitterDeg);
        }
    }
}