using System;
using _Project.Runtime.Abstract.MVP;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Abstract.Weapons
{
    public abstract class BaseWeapon<TWeaponConfig> : IFixedTickable where TWeaponConfig : WeaponConfig
    {
        protected readonly TWeaponConfig Config;
        protected readonly IFireParamsSource Source;
        
        protected float Cooldown;
        

        protected BaseWeapon(TWeaponConfig config, IFireParamsSource source)
        {
            Config = config;
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

        public abstract bool TryAttack();
    }
}