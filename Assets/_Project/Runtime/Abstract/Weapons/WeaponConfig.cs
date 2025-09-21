using UnityEngine;

namespace _Project.Runtime.Abstract.Weapons
{
    public abstract class WeaponConfig : ScriptableObject
    {
        [field: SerializeField]
        public float WeaponCooldown { get; private set; }

        [field: SerializeField]
        public float MuzzleOffset { get; private set; }
    }
}