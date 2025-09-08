using Runtime.Weapons;

namespace Runtime.Abstract.Weapons
{
    public interface IProjectileWeaponConfig : IWeaponConfig
    {
        ProjectileConfig Projectile { get; }
        float Spread { get; }
        int BulletsPerShot { get; }
        float BulletsInterval { get; }
    }
}