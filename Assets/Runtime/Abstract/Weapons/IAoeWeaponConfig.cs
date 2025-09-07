using Runtime.Settings;
using Runtime.Weapons;

namespace Runtime.Abstract.Weapons
{
    public interface IAoeWeaponConfig : IWeaponConfig
    {
        AoeAttackConfig Attack { get; }
        int Charges { get; }
        float ChargeRate { get; }
    }
}