using Runtime.Abstract.Configs;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ChasingEnemyConfig", menuName = "Settings/Chasing Enemy Config")]
    public sealed class ChasingEnemyConfig : ScriptableObject, IChasingEnemyConfig
    {
        [Header("Turn / Thrust")]
        [SerializeField,
         Tooltip("Proportional gain for heading error (radians). Higher = snappier turns, too high = oscillations.")]
        private float _turnProportionalGainKp = 2.2f;

        [SerializeField,
         Tooltip("Derivative gain (damping) for heading error rate. Higher = less overshoot, too high = sluggish.")]
        private float _turnDerivativeGainKd = 0.8f;

        [SerializeField,
         Tooltip("Proportional gain for thrust based on distance to target. Higher = more aggressive chase.")]
        private float _thrustProportionalGainKp = 0.35f;

        [SerializeField, Range(0f, 1f),
         Tooltip("Maximum normalized thrust to apply [0..1]. Acts as a hard cap for acceleration.")]
        private float _thrustMaxNormalized = 1f;

        [SerializeField, Tooltip("Angular tolerance (in degrees) within which the enemy is allowed to fire.")]
        private float _aimToleranceDegrees = 6f;


        [Header("Fire")]
        [SerializeField, Min(0f),
         Tooltip("Maximum time used for aim prediction (seconds). Limits how far ahead we lead the target.")]
        private float _aimMaxPredictionLeadSeconds = 1.2f;

        [SerializeField, Min(0f), Tooltip("Do not fire when the target is farther than this distance (world units).")]
        private float _maxFireDistanceUnits = 20f;
        
        public float TurnKp => _turnProportionalGainKp;
        public float TurnKd => _turnDerivativeGainKd;
        public float ThrustKp => _thrustProportionalGainKp;
        public float MaxThrust => _thrustMaxNormalized;
        public float AimToleranceDegrees => _aimToleranceDegrees;

        public float MaxLeadSeconds => _aimMaxPredictionLeadSeconds;
        public float MaxFireDistance => _maxFireDistanceUnits;

        private void OnValidate()
        {
            _thrustMaxNormalized = Mathf.Clamp01(_thrustMaxNormalized);
            _aimToleranceDegrees = Mathf.Max(0f, _aimToleranceDegrees);
            _aimMaxPredictionLeadSeconds = Mathf.Max(0f, _aimMaxPredictionLeadSeconds);
            _maxFireDistanceUnits = Mathf.Max(0f, _maxFireDistanceUnits);
        }
    }
}