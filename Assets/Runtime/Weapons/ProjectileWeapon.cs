using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Weapons
{
    public class ProjectileWeapon : BaseWeapon<IProjectileWeaponConfig>
    {
        private int _pendingInBurst;
        private float _burstTimer;
        
        public ProjectileWeapon(IProjectileWeaponConfig config, IFireParamsSource source) : base(config, source)
        { }

        public override bool TryAttack()
        {
            if (Cooldown > 0f)
            {
                return false;
            }

            if (Source == null ||
                !Source.TryGetFireParams(out var origin, out var dir, out var inheritVelocity, out int layer))
            {
                return false;
            }

            dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.up;
            origin += dir * Config.MuzzleOffset;

            int count = Mathf.Max(1, Config.BulletsPerShot);

            if (Config.BulletsInterval <= 0f || count == 1)
            {
                for (var i = 0; i < count; i++)
                    NotifyAttack(ComposeShot(origin, ApplySpread(dir), inheritVelocity, layer));
            }
            else
            {
                NotifyAttack(ComposeShot(origin, ApplySpread(dir), inheritVelocity, layer));
                _pendingInBurst = count - 1;
                _burstTimer = Config.BulletsInterval;
            }

            Cooldown = Mathf.Max(0f, Config.WeaponCooldown);
            return true;
        }

        protected override void OnFixedTick()
        {
            if (_pendingInBurst <= 0)
            {
                return;
            }

            _burstTimer -= Time.fixedDeltaTime;

            while (_pendingInBurst > 0 && _burstTimer <= 0f)
            {
                if (Source == null ||
                    !Source.TryGetFireParams(out var origin, out var dir, out var inheritVelocity, out int layer))
                    break;

                dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.up;
                origin += dir * Config.MuzzleOffset;

                NotifyAttack(ComposeShot(origin, ApplySpread(dir), inheritVelocity, layer));

                _pendingInBurst--;
                _burstTimer += Config.BulletsInterval;
            }
        }

        private Vector2 ApplySpread(Vector2 dir)
        {
            float half = 0.5f * Config.Spread * Mathf.Deg2Rad;
            float a = Random.Range(-half, half);
            return GeometryMethods.RotateVector(dir, a).normalized;
        }

        private ProjectileShoot ComposeShot(Vector2 origin, Vector2 dir, Vector2 inherit, int layer)
        {
            return new ProjectileShoot(
                Config.Projectile,
                origin,
                dir,
                inherit,
                layer
            );
        }
    }
}