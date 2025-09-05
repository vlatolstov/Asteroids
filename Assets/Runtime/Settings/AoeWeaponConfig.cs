using Runtime.Abstract.Configs;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "LaserConfig", menuName = "Settings/Weapons/Laser Weapon", order = 0)]
    public class AoeWeaponConfig : WeaponConfig, IAoeWeaponConfig
    {

        [SerializeField]
        private float _laserLength = 2f;

        [SerializeField]
        private float _laserWidth = 0.5f;

        [SerializeField]
        private int _charges = 3;

        [SerializeField]
        private float _chargeRate = 5f;
        public float Length => _laserLength;
        public float StartWidth => _laserWidth;
        public float EndWidth { get; }
        public float Duration { get; }
        public int Charges => _charges;
        public float ChargeRate => _chargeRate;
    }
}