namespace Runtime.Abstract.Configs
{
    public interface IWeaponConfig
    {
        float WeaponCooldown { get; }
        float MuzzleOffset { get; }
    }
}