using System;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    public class AoeWeapon : BaseWeapon<AoeWeaponConfig>
    {
        private int _charges;
        private float _rechargeTime;

        public event Action<AoeAttackReleased> AttackReleased;

        public AoeWeapon(AoeWeaponConfig config, IFireParamsSource source) : base(config, source)
        {
            Reinforce();
        }

        public void Reinforce()
        {
            Cooldown = 0; 
            _charges = Config.Charges;
            _rechargeTime = Config.ChargeRate;
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
            var attack = new AoeAttackReleased(originTransform, Config, sourceType);
            AttackReleased?.Invoke(attack);

            Cooldown = Mathf.Max(0f, Config.WeaponCooldown);
        }

        protected override void OnFixedTick()
        {
            if (_charges >= Config.Charges)
            {
                return;
            }

            if (_rechargeTime <= 0)
            {
                _charges++;
                _rechargeTime = Config.ChargeRate;
            }
            else
            {
                _rechargeTime -= Time.fixedDeltaTime;
            }
        }

        public AoeWeaponState ProvideAoeWeaponState()
        {
            var recharge = 1f - Mathf.Clamp01(_rechargeTime / Mathf.Max(Config.ChargeRate, 1e-6f));
            var reload = 1f - Mathf.Clamp01(Cooldown / Mathf.Max(Config.WeaponCooldown, 1e-6f));
            return new AoeWeaponState(Config.Charges, _charges, Cooldown,reload, recharge);
        }
    }
}