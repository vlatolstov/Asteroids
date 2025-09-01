using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Settings/MovementConfig", order = 0)]
    public class MovementConfig : ScriptableObject, IMovementConfig
    {
        [SerializeField]
        private float _acceleration = 1f;

        [SerializeField]
        private float _maxSpeed = 10f;

        [SerializeField]
        private float _turnSpeed = 1f;

        [SerializeField]
        private float _linearDamping = 0.2f;

        [SerializeField]
        private bool _isWrappedByWorldBounds = false;

        public float Acceleration => _acceleration;
        public float MaxSpeed => _maxSpeed;
        public float TurnSpeed => _turnSpeed;
        public float LinearDamping => _linearDamping;
        public bool IsWrappedByWorldBounds => _isWrappedByWorldBounds;
    }
}