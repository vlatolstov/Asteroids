using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "BackgroundJitterConfig", menuName = "Settings/Background Jitter Config", order = 0)]
    public class BackgroundJitterConfig : ScriptableObject
    {
        [Header("Random drift")]
        [field: SerializeField]
        public float SlowAmplitude { get; private set; } = 0.6f;

        [field: SerializeField]
        public float SlowFrequency { get; private set; } = 0.05f;

        [field: SerializeField]
        public float JitterAmplitude { get; private set; } = 0.03f;

        [field: SerializeField]
        public float JitterFrequency { get; private set; } = 8f;

        [Header("Parallax vs player velocity")]
        [field: SerializeField]
        public float ParallaxStrength { get; private set; } = 0.3f;

        [field: SerializeField]
        public float ParallaxResponse { get; private set; } = 6f;

        [field: SerializeField]
        public float ParallaxMax { get; private set; } = 500f;

        [Header("Smoothing / Limits")]
        [field: SerializeField]
        public float SmoothTime { get; private set; } = 0.2f;

        [field: SerializeField]
        public float MaxOffset { get; private set; } = 500f;

        [field: SerializeField]
        public bool UseUnscaledTime { get; private set; }
    }
}