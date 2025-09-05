namespace Runtime.Abstract.Weapons
{
    public interface IAoeWeaponConfig : IWeaponConfig
    {
        float Length { get; }
        float StartWidth { get; }
        float EndWidth { get; }
        float Duration { get; }
        int Charges { get; }
        float ChargeRate { get; }
    }
}