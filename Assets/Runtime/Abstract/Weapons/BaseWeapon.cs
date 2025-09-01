using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Abstract.Weapons
{
    public abstract class BaseWeapon : IInitializable, IFixedTickable, IDisposable
    {
        private readonly ProjectileHitResolver _resolver;
        protected readonly IGunConfig Config;
        protected readonly BulletView.Pool BulletPool;

        private float _cooldown;

        protected BaseWeapon(IGunConfig config, BulletView.Pool bulletPool, ProjectileHitResolver resolver)
        {
            Config = config;
            BulletPool = bulletPool;
            _resolver = resolver;
        }

        public virtual void Initialize()
        { }

        public virtual void Dispose()
        { }

        public void FixedTick()
        {
            if (_cooldown > 0f)
            {
                _cooldown = Mathf.Max(0f, _cooldown - Time.fixedDeltaTime);
            }

            OnFixedTick();
        }

        protected virtual void OnFixedTick()
        { }
        
        public bool TryFire()
        {
            if (_cooldown > 0f) return false;

            if (!GetFireParams(out var origin, out var dir, out var inheritVel, out var faction))
                return false;

            var vel = inheritVel + dir.normalized * Config.BulletSpeed;

            var bullet = BulletPool.Spawn(origin, vel, Config.BulletLife, faction);
            bullet.Emitted += OnHitEvent;

            _cooldown = Config.BulletCooldown;
            return true;
        }

        private void OnHitEvent(IData data)
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