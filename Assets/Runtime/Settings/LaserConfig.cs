using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "LaserConfig", menuName = "Settings/Weapons/LaserConfig", order = 0)]
    public class LaserConfig : ScriptableObject, ILaserConfig
    {
        [SerializeField]
        private float _weaponCooldown = 1f;

        [SerializeField]
        private float _muzzleOffset = 0f;

        [SerializeField]
        private float _laserLength = 2f;

        [SerializeField]
        private float _laserWidth = 0.5f;

        [SerializeField]
        private int _charges = 3;

        [SerializeField]
        private float _chargeRate = 5f;

        public float WeaponCooldown => _weaponCooldown;
        public float MuzzleOffset => _muzzleOffset;
        public float LaserLength => _laserLength;
        public float LaserWidth => _laserWidth;
        public int Charges => _charges;
        public float ChargeRate => _chargeRate;
    }
}