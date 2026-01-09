using System;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Weapons
{
    public class ProjectileWeapon : BaseWeapon<ProjectileWeaponConfig>
    {
        private readonly ProjectileWeaponData _data;
        private readonly ProjectileAttackData _attackData;

        private int _pendingInBurst;
        private float _burstTimer;

        public event Action<ProjectileShot> ProjectileFired;

        public ProjectileWeapon(ProjectileWeaponConfig config, ProjectileWeaponData data,
            ProjectileAttackData attackData, IFireParamsSource source) : base(config, source)
        {
            _data = data ?? new ProjectileWeaponData();
            _attackData = attackData ?? new ProjectileAttackData();
        }

        public virtual void Attack()
        {
            if (Cooldown > 0f)
            {
                return;
            }

            if (Source == null ||
                !Source.TryGetFireParams(out var origin, out var dir, out var inheritVelocity, out int layer,
                    out var sourceType))
            {
                return;
            }

            dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.up;
            origin += dir * _data.MuzzleOffset;

            int count = Mathf.Max(1, _data.BulletsPerShot);

            if (_data.BulletsInterval <= 0f || count == 1)
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
                _burstTimer = _data.BulletsInterval;
            }

            Cooldown = Mathf.Max(0f, _data.WeaponCooldown);
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
                origin += dir * _data.MuzzleOffset;

                NotifyAttack(origin, ApplySpread(dir), inheritVelocity, layer, sourceType);

                _pendingInBurst--;
                _burstTimer += _data.BulletsInterval;
            }
        }

        private Vector2 ApplySpread(Vector2 dir)
        {
            float half = 0.5f * _data.Spread * Mathf.Deg2Rad;
            float a = Random.Range(-half, half);
            return GeometryMethods.RotateVector(dir, a).normalized;
        }

        private void NotifyAttack(Vector2 origin, Vector2 dir, Vector2 inheritVelocity, int layer, Source sourceType)
        {
            var attack = new ProjectileShot(origin, Quaternion.LookRotation(dir), 
                scale: Vector2.one, ApplySpread(dir),
                inheritVelocity, layer, Config, _attackData, sourceType);
            
            ProjectileFired?.Invoke(attack);
        }

        public ProjectileWeaponState ProvideProjWeaponState()
        {
            var reload = 1f - Mathf.Clamp01(Cooldown / Mathf.Max(_data.WeaponCooldown, 1e-6f));
            return new ProjectileWeaponState(Cooldown, reload);
        }
    }
}
