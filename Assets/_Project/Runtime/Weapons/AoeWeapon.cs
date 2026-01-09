using System;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using _Project.Runtime.RemoteConfig;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    public class AoeWeapon : BaseWeapon<AoeWeaponConfig>
    {
        private readonly AoeWeaponData _data;
        private readonly AoeAttackData _attackData;

        private int _charges;
        private float _rechargeTime;

        public event Action<AoeAttackReleased> AttackReleased;

        public AoeWeapon(AoeWeaponConfig config, AoeWeaponData data, AoeAttackData attackData, IFireParamsSource source)
            : base(config, source)
        {
            _data = data ?? new AoeWeaponData();
            _attackData = attackData ?? new AoeAttackData();
            Reinforce();
        }

        public void Reinforce()
        {
            Cooldown = 0; 
            _charges = _data.Charges;
            _rechargeTime = _data.ChargeRate;
        }

        public virtual void Attack()
        {
            if (Cooldown > 0f || _charges <= 0)
            {
                return;
            }

            if (Source is not MonoBehaviour mb ||
                !Source.TryGetFireParams(out _, out _, out _, out _, out var sourceType))
            {
                return;
            }

            var originTransform = mb.transform;

            _charges--;
            var attack = new AoeAttackReleased(originTransform, Config, _data, _attackData, sourceType);
            AttackReleased?.Invoke(attack);

            Cooldown = Mathf.Max(0f, _data.WeaponCooldown);
        }

        protected override void OnFixedTick()
        {
            if (_charges >= _data.Charges)
            {
                return;
            }

            if (_rechargeTime <= 0)
            {
                _charges++;
                _rechargeTime = _data.ChargeRate;
            }
            else
            {
                _rechargeTime -= Time.fixedDeltaTime;
            }
        }

        public AoeWeaponState ProvideAoeWeaponState()
        {
            var recharge = 1f - Mathf.Clamp01(_rechargeTime / Mathf.Max(_data.ChargeRate, 1e-6f));
            var reload = 1f - Mathf.Clamp01(Cooldown / Mathf.Max(_data.WeaponCooldown, 1e-6f));
            return new AoeWeaponState(_data.Charges, _charges, Cooldown, reload, recharge);
        }
    }
}
