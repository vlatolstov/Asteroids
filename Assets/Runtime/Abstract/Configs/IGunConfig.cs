namespace Runtime.Abstract.Configs
{
    public interface IGunConfig : IWeaponConfig
    {
        public float BulletSpeed { get; }
        public float BulletLife { get; }
        public int BulletsPerShot { get; }
        public float BulletsInterval { get; }
    }
}