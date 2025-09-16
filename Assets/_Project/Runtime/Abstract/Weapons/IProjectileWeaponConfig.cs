using _Project.Runtime.Weapons;

namespace _Project.Runtime.Abstract.Weapons
{
    public interface IProjectileWeaponConfig : IWeaponConfig
    {
        ProjectileConfig Projectile { get; }
        float Spread { get; }
        int BulletsPerShot { get; }
        float BulletsInterval { get; }
    }
}