namespace Runtime.Abstract.Configs
{
    public interface IGunConfig : IWeaponConfig
    {
        float BulletSpeed { get; }
        float BulletLife { get; }
        int BulletsPerShot { get; }
        float BulletsInterval { get; }
    }
}