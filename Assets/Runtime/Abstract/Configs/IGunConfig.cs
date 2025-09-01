namespace Runtime.Abstract.Configs
{
    public interface IGunConfig
    {
        public float BulletSpeed { get; }
        public float BulletCooldown { get; }
        public float BulletLife { get; }
        public float MuzzleOffset { get; }
    }
}