using _Project.Runtime.Abstract.Weapons;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "AoeWeaponResource", menuName = "Resources/Weapons/AOE Weapon", order = 0)]
    public class AoeWeaponResource : WeaponResource
    {
        [Header("Logic")]
        [field: SerializeField]
        public AoeAttackResource Attack { get; private set; }

        [Header("Representation")]
        [SerializeField]
        private AudioClip[] _attackSounds;

        public AudioClip AttackSound => _attackSounds[Random.Range(0, _attackSounds.Length)];
    }
}
