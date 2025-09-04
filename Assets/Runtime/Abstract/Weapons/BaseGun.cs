using Runtime.Abstract.Configs;
using Runtime.Data;
using Runtime.Views;
using Runtime.Weapons;
using UnityEngine;

namespace Runtime.Abstract.Weapons
{
    public class BaseGun : BaseWeapon<IProjectileWeaponConfig>
    {
        private readonly ProjectileView.Pool _bulletPool;

        public BaseGun(IProjectileWeaponConfig config, ProjectileHitResolver resolver, ProjectileView.Pool pool) : base(config, resolver)
        {
            _bulletPool = pool;
        }

        public override bool TryFire()
        {
            return true;
            
            // if (Cooldown > 0f) return false;
            // if (!GetFireParams(out var origin, out var dir, out var inheritVel, out var faction))
            //     return false;
            //
            // var vel = inheritVel + dir.normalized * Config.BulletSpeed;
            //
            // var bullet = _bulletPool.Spawn(origin, vel, Config.BulletLife, faction);
            // bullet.Emitted += OnHitEvent;
            //
            // Cooldown = Config.WeaponCooldown;
            // return true;
        }

        protected override bool GetFireParams(out Vector2 origin, out Vector2 direction, out Vector2 inheritVelocity)
        {
            throw new System.NotImplementedException();
        }
    }
}