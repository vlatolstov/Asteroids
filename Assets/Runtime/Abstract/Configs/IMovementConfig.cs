namespace Runtime.Abstract.Configs
{
    public interface IMovementConfig
    {
        float Acceleration { get; }
        float MaxSpeed { get; }
        float TurnSpeed { get; }
        float LinearDamping { get; }
        bool IsWrappedByWorldBounds { get; }
    }
}