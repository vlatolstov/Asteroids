using Runtime.Abstract.Configs;

namespace Runtime.Abstract.Weapons
{
    public interface IProjectileWeaponConfig : IWeaponConfig
    {
        IProjectileConfig Projectile { get; }
        float Spread { get; }
        int BulletsPerShot { get; }
        float BulletsInterval { get; }
    }
}