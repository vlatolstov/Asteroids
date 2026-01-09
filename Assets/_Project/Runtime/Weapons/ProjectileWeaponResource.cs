using _Project.Runtime.Abstract.Weapons;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "ProjectileWeaponResource", menuName = "Resources/Weapons/Projectile Weapon", order = 0)]
    public class ProjectileWeaponResource : WeaponResource
    {
        [Header("Logic")]
        [field: SerializeField]
        public ProjectileResource Projectile { get; private set; }

        [Header("Representation")]
        [SerializeField]
        private AudioClip[] _attackSounds;

        public AudioClip AttackSound => _attackSounds[Random.Range(0, _attackSounds.Length)];
    }
}
