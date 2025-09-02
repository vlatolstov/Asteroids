using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Abstract.Weapons
{
    public abstract class BaseWeapon<TWeaponConfig> : IInitializable, IFixedTickable, IDisposable where TWeaponConfig : IWeaponConfig
    {
        private readonly ProjectileHitResolver _resolver;
        protected readonly TWeaponConfig Config;

        protected float Cooldown;

        protected BaseWeapon(TWeaponConfig config, ProjectileHitResolver resolver)
        {
            Config = config;
            _resolver = resolver;
        }

        public virtual void Initialize()
        { }

        public virtual void Dispose()
        { }

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

        public abstract bool TryFire();
        
        protected void OnHitEvent(IData data)
        {
            if (data is ProjectileHit hit)
            {
                _resolver.Handle(hit);
            }
        }

        protected abstract bool GetFireParams(out Vector2 origin, out Vector2 direction, out Vector2 inheritVelocity,
            out Faction faction);
    }
}