using Runtime.Abstract.Visualization;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Weapons
{
    [CreateAssetMenu(fileName = "AoeWeaponConfig", menuName = "Settings/Weapons/AOE Weapon", order = 0)]
    public class AoeWeaponConfig : WeaponConfig, IAoeWeaponConfig, IWeaponRepresentation
    {
        [Header("Logic")]
        [SerializeField]
        private AoeAttackConfig _attack;
        
        [SerializeField]
        private int _charges = 3;

        [SerializeField]
        private float _chargeRate = 5f;

        [Header("Representation")]
        [SerializeField]
        private AudioClip[] _attackSounds;

        public AoeAttackConfig Attack => _attack;
        public int Charges => _charges;
        public float ChargeRate => _chargeRate;
        public AudioClip AttackSound => _attackSounds[Random.Range(0, _attackSounds.Length)];
    }
}