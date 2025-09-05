using UnityEngine;

namespace Runtime.Abstract.Configs
{
    public interface IAsteroidsSpawnConfig
    {
        Sprite Sprite { get; }
        float AngleRotationDeg { get; }
        float Interval { get; }
        float LargeScale { get; }
        float EdgeOffset { get; }
        float LargeSpeed { get; }
        float EntryAngleJitterDeg { get; }
        float SmallScale { get; }
        int SmallSplit { get; }
        float SmallSpeed { get; }
    }
}