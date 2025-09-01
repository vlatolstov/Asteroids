namespace Runtime.Abstract.Configs
{
    public interface ILaserConfig : IWeaponConfig
    {
        public float LaserLength { get; }
        public float LaserWidth { get; }
        public int Charges { get; }
        public float ChargeRate { get; }
    }
}