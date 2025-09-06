using Runtime.Abstract.Visualization;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ProjectileWeapon", menuName = "Settings/Weapons/Projectile Weapon", order = 0)]
    public class ProjectileWeaponConfig : WeaponConfig, IProjectileWeaponConfig, IWeaponRepresentation
    {
        [Header("Logic")]
        [SerializeField]
        private ProjectileConfig _projectileConfig;

        [SerializeField]
        private float _spread = 1f;

        [SerializeField]
        private int _bulletsPerShot = 1;

        [SerializeField]
        private float _bulletsInterval = 1f;

        [Header("Representation")]
        [SerializeField]
        private Sprite _weaponSprite;

        [SerializeField]
        private AudioClip _attackSound;

        public ProjectileConfig Projectile => _projectileConfig;

        public float Spread => _spread;
        public int BulletsPerShot => _bulletsPerShot;
        public float BulletsInterval => _bulletsInterval;
        public Sprite WeaponSprite => _weaponSprite;
        public AudioClip AttackSound => _attackSound;
    }
}