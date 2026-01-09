using UnityEngine;
using Zenject;

namespace _Project.Runtime.Abstract.Weapons
{
    public abstract class BaseWeapon<TWeaponResource> : IFixedTickable where TWeaponResource : WeaponResource
    {
        protected readonly TWeaponResource Resource;
        protected readonly IFireParamsSource Source;
        
        protected float Cooldown;
        

        protected BaseWeapon(TWeaponResource resource, IFireParamsSource source)
        {
            Resource = resource;
            Source = source;
        }

        public void FixedTick()
        {
            if (Cooldown > 0f)
            {
                Cooldown = Mathf.Max(0f, Cooldown - Time.fixedDeltaTime);
            }

            OnFixedTick();
        }

        protected virtual void OnFixedTick()
        { }
    }
}
