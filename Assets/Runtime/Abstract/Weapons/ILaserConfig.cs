using Runtime.Abstract.Weapons;

namespace Runtime.Abstract.Configs
{
    public interface ILaserConfig : IWeaponConfig
    {
        float LaserLength { get; }
        float LaserWidth { get; }
        int Charges { get; }
        float ChargeRate { get; }
    }
}