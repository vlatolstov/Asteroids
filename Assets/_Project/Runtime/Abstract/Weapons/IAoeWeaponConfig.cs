using _Project.Runtime.Weapons;

namespace _Project.Runtime.Abstract.Weapons
{
    public interface IAoeWeaponConfig : IWeaponConfig
    {
        AoeAttackConfig Attack { get; }
        int Charges { get; }
        float ChargeRate { get; }
    }
}