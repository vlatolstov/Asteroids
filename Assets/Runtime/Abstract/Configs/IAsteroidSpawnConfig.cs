namespace Runtime.Abstract.Configs
{
    public interface IAsteroidsSpawnConfig
    {
        float Interval { get; }
        float EdgeOffset { get; }
        float EntrySpeedMin { get; }
        float EntrySpeedMax { get; }
        float EntryAngleJitterDeg { get; }
        int   SmallSplitMin { get; }
        int   SmallSplitMax { get; }
        float SmallSpeedMin { get; }
        float SmallSpeedMax { get; }
    }
}