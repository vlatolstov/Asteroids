using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ShipConfig", menuName = "Settings/ShipConfig", order = 0)]
    public class ShipConfig : ScriptableObject
    {
        [Header("Ship")]
        public float Acceleration = 1f;

        public float MaxSpeed = 10f;
        public float TurnSpeed = 1f;
        public float Radius = 1f;

        [Header("Bullets")]
        public float BulletLife = 1f;

        public float BulletSpeed = 15f;
        public float BulletCooldown = 1f;

        [Header("Laser")]
        public int MaxLaserCharges = 3;
        public float LaserRechargeRate = 8f;
        public float LaserCooldown = 3f;
        public float LaserRange = 6f;
    }
}