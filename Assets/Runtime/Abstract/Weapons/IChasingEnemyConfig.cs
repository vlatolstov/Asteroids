namespace Runtime.Abstract.Weapons
{
    public interface IChasingEnemyConfig
    {
        public interface IChasingEnemyConfig
        {
            float TurnKp { get; }
            float TurnKd { get; }
            float ThrustKp { get; }
            float MaxThrust { get; }
            float AimToleranceDegrees { get; }

            float MaxLeadSeconds { get; }
            float MaxFireDistance { get; }
        }
    }
}