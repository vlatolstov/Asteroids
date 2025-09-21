using _Project.Runtime.Abstract.Weapons;
using UnityEngine;

namespace _Project.Runtime.Settings
{
    [CreateAssetMenu(fileName = "ChasingEnemyConfig", menuName = "Settings/Chasing Enemy Config")]
    public sealed class ChasingEnemyConfig : ScriptableObject
    {
        [Header("Turn / Thrust")]
        [field: SerializeField,
                Tooltip(
                    "Proportional gain for heading error (radians). Higher = snappier turns, too high = oscillations.")]
        public float TurnKp { get; private set; }

        [field: SerializeField,
                Tooltip(
                    "Derivative gain (damping) for heading error rate. Higher = less overshoot, too high = sluggish.")]
        public float TurnKd { get; private set; }

        [field: SerializeField,
                Tooltip("Proportional gain for thrust based on distance to target. Higher = more aggressive chase.")]
        public float ThrustKp { get; private set; }

        [field: SerializeField, Range(0f, 1f),
                Tooltip("Maximum normalized thrust to apply [0..1]. Acts as a hard cap for acceleration.")]
        public float MaxThrust { get; private set; }

        [field: SerializeField, Tooltip("Angular tolerance (in degrees) within which the enemy is allowed to fire.")]
        public float AimToleranceDegrees { get; private set; }


        [Header("Fire")]
        [field: SerializeField, Min(0f),
                Tooltip("Maximum time used for aim prediction (seconds). Limits how far ahead we lead the target.")]
        public float MaxLeadSeconds { get; private set; }

        [field: SerializeField, Min(0f),
                Tooltip("Do not fire when the target is farther than this distance (world units).")]
        public float MaxFireDistance { get; private set; }


        private void OnValidate()
        {
            MaxThrust = Mathf.Clamp01(MaxThrust);
            AimToleranceDegrees = Mathf.Max(0f, AimToleranceDegrees);
            MaxLeadSeconds = Mathf.Max(0f, MaxLeadSeconds);
            MaxFireDistance = Mathf.Max(0f, MaxFireDistance);
        }
    }
}