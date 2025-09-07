using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Settings;
using UnityEngine;

namespace Runtime.Weapons
{
    public class AoeWeapon : BaseWeapon<AoeWeaponConfig>
    {
        private int _charges;
        private float _rechargeTime;

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

        public override bool TryAttack()
        {
            if (Cooldown > 0f || _charges <= 0)
            {
                return false;
            }

            if (Source is not MonoBehaviour mb)
            {
                return false;
            }

            var originTransform = mb.transform;

            _charges--;
            NotifyAttack(ComposeAttack(originTransform, Config));

            Cooldown = Mathf.Max(0f, Config.WeaponCooldown);
            return true;
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

        private AoeAttackReleased ComposeAttack(Transform origin, AoeWeaponConfig weapon)
        {
            return new AoeAttackReleased(
                origin,
                weapon
            );
        }

        public AoeWeaponState ProvideAoeWeaponState()
        {
            var recharge = 1f - Mathf.Clamp01(_rechargeTime / Mathf.Max(Config.ChargeRate, 1e-6f));
            return new AoeWeaponState(Config.Charges, _charges, Cooldown,recharge);
        }
    }
}