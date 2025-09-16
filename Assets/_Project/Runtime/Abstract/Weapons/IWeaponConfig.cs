namespace _Project.Runtime.Abstract.Weapons
{
    public interface IWeaponConfig
    {
        float WeaponCooldown { get; }
        float MuzzleOffset { get; }
    }
}