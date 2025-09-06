using Runtime.Abstract.MVP;

namespace Runtime.Data
{
    public struct BulletState : IStateData
    {
        public float Cooldown;

        public BulletState(float cooldown)
        {
            Cooldown = cooldown;
        }
    }

    public struct AoeWeaponState : IStateData
    {
        public int Charges;
        public float RechargeRatio;
        public float Cooldown;

        public AoeWeaponState(int charges, float cooldown, float rechargeRatio)
        {
            Charges = charges;
            Cooldown = cooldown;
            RechargeRatio = rechargeRatio;
        }
    }
}