using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ShipConfig", menuName = "Settings/ShipConfig", order = 0)]
    public class ShipConfig : ScriptableObject, IMovementConfig
    {
        [Header("Ship")]
        [SerializeField]
        private float _acceleration = 1f;

        [SerializeField]
        private float _maxSpeed = 10f;

        [SerializeField]
        private float _turnSpeed = 1f;

        [SerializeField]
        private float _linearDamping = 0.2f;

        [Header("Bullets")]
        public float BulletLife = 1f;

        public float BulletSpeed = 15f;
        public float BulletCooldown = 1f;

        [Header("Laser")]
        public int MaxLaserCharges = 3;

        public float LaserRechargeRate = 8f;
        public float LaserCooldown = 3f;
        public float LaserRange = 6f;
        public float Acceleration => _acceleration;
        public float MaxSpeed => _maxSpeed;
        public float TurnSpeed => _turnSpeed;
        public float LinearDamping => _linearDamping;
    }
}