using UnityEngine;

namespace Runtime.Abstract.Configs
{
    public interface IAsteroidsSpawnConfig
    {
        float Interval { get; }
        float LargeScale { get; }
        float EdgeOffset { get; }
        float EntrySpeedMin { get; }
        float EntrySpeedMax { get; }
        float EntryAngleJitterDeg { get; }
        float SmallScale { get; }
        int SmallSplitMin { get; }
        int SmallSplitMax { get; }
        float SmallSpeedMin { get; }
        float SmallSpeedMax { get; }
    }
}