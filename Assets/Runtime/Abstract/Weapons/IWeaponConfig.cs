namespace Runtime.Abstract.Weapons
{
    public interface IWeaponConfig
    {
        float WeaponCooldown { get; }
        float MuzzleOffset { get; }
    }
}