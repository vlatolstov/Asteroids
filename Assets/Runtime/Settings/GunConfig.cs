using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "GunConfig", menuName = "Settings/Weapons/GunConfig", order = 0)]
    public class GunConfig : ScriptableObject, IGunConfig
    {
        [SerializeField]
        private float _weaponCooldown = 1f;

        [SerializeField]
        private float _muzzleOffset = 0f;

        [SerializeField]
        private float _bulletSpeed = 15f;

        [SerializeField]
        private float _bulletLife = 1f;

        [SerializeField]
        private int _bulletsPerShot = 1;

        [SerializeField]
        private float _bulletsInterval = 1f;


        public float WeaponCooldown => _weaponCooldown;
        public float MuzzleOffset => _muzzleOffset;
        public float BulletSpeed => _bulletSpeed;
        public float BulletLife => _bulletLife;
        public int BulletsPerShot => _bulletsPerShot;
        public float BulletsInterval => _bulletsInterval;
    }
}