using Runtime.Settings;

namespace Runtime.Abstract.Weapons
{
    public interface IAoeAttackConfig
    {
        float Length { get; }
        float Width { get; }
        float Duration { get; }
    }
}