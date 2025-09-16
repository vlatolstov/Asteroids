using _Project.Runtime.Abstract.MVP;

namespace _Project.Runtime.Data
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
        public int MaxCharges;
        public int Charges;
        public float RechargeRatio;
        public float Cooldown;

        public AoeWeaponState(int maxCharges, int charges, float cooldown, float rechargeRatio)
        {
            MaxCharges = maxCharges;
            Charges = charges;
            Cooldown = cooldown;
            RechargeRatio = rechargeRatio;
        }
    }
}