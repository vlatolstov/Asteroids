using Runtime.Abstract.Visualization;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
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
        private Sprite _weaponSprite;

        [SerializeField]
        private AudioClip _attackSound;

        public AoeAttackConfig Attack => _attack;
        public int Charges => _charges;
        public float ChargeRate => _chargeRate;
        public Sprite WeaponSprite => _weaponSprite;
        public AudioClip AttackSound => _attackSound;
    }
}