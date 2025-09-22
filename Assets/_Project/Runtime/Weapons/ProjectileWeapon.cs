using System;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using _Project.Runtime.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Weapons
{
    public class ProjectileWeapon : BaseWeapon<ProjectileWeaponConfig>
    {
        private int _pendingInBurst;
        private float _burstTimer;

        public event Action<ProjectileShot> ProjectileFired;

        public ProjectileWeapon(ProjectileWeaponConfig config, IFireParamsSource source) : base(config, source)
        { }

        public override bool TryAttack()
        {
            if (Cooldown > 0f)
            {
                return false;
            }

            if (Source == null ||
                !Source.TryGetFireParams(out var origin, out var dir, out var inheritVelocity, out int layer,
                    out var sourceType))
            {
                return false;
            }

            dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.up;
            origin += dir * Config.MuzzleOffset;

            int count = Mathf.Max(1, Config.BulletsPerShot);

            if (Config.BulletsInterval <= 0f || count == 1)
            {
                for (var i = 0; i < count; i++)
                {
                    NotifyAttack(origin, ApplySpread(dir), inheritVelocity, layer, sourceType);
                }
            }
            else
            {
                NotifyAttack(origin, ApplySpread(dir), inheritVelocity, layer, sourceType);
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
                    !Source.TryGetFireParams(out var origin, out var dir, out var inheritVelocity, out int layer,
                        out var sourceType))
                    break;

                dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.up;
                origin += dir * Config.MuzzleOffset;

                NotifyAttack(origin, ApplySpread(dir), inheritVelocity, layer, sourceType);

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

        private void NotifyAttack(Vector2 origin, Vector2 dir, Vector2 inheritVelocity, int layer, Source sourceType)
        {
            var attack = new ProjectileShot(origin, Quaternion.LookRotation(dir), 
                scale: Vector2.one, ApplySpread(dir),
                inheritVelocity, layer, Config, sourceType);
            
            ProjectileFired?.Invoke(attack);
        }

        public ProjectileWeaponState ProvideProjWeaponState()
        {
            return new ProjectileWeaponState(Cooldown, 1 - Cooldown / Config.WeaponCooldown);
        }
    }
}