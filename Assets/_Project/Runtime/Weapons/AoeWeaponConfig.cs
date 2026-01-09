using _Project.Runtime.Abstract.Weapons;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "AoeWeaponConfig", menuName = "Settings/Weapons/AOE Weapon", order = 0)]
    public class AoeWeaponConfig : WeaponConfig
    {
        [Header("Logic")]
        [field: SerializeField]
        public AoeAttackConfig Attack { get; private set; }

        [Header("Representation")]
        [SerializeField]
        private AudioClip[] _attackSounds;

        public AudioClip AttackSound => _attackSounds[Random.Range(0, _attackSounds.Length)];
    }
}
