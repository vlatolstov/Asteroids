using _Project.Runtime.Abstract.Weapons;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "ProjectileWeapon", menuName = "Settings/Weapons/Projectile Weapon", order = 0)]
    public class ProjectileWeaponConfig : WeaponConfig
    {
        [Header("Logic")]
        [field: SerializeField]
        public ProjectileConfig Projectile { get; private set; }

        [field: SerializeField]
        public float Spread { get; private set; }

        [field: SerializeField]
        public int BulletsPerShot { get; private set; }

        [field: SerializeField]
        public float BulletsInterval { get; private set; }

        [Header("Representation")]
        [SerializeField]
        private AudioClip[] _attackSounds;

        public AudioClip AttackSound => _attackSounds[Random.Range(0, _attackSounds.Length)];
    }
}