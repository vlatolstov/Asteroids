using UnityEngine;

namespace _Project.Runtime.Abstract.Weapons
{
    public abstract class WeaponConfig : ScriptableObject, IWeaponConfig
    {
        [SerializeField]
        private float _weaponCooldown = 1f;

        [SerializeField]
        private float _muzzleOffset = 0f;
        
        public float WeaponCooldown => _weaponCooldown;
        public float MuzzleOffset => _muzzleOffset;
    }
}