using UnityEngine;

namespace Runtime.Abstract.Configs
{
    public interface IUfoSpawnConfig
    {
        Sprite Sprite { get; }
        float Scale { get; }

        float InitialDelay { get; }
        float Interval { get; }
        int MaxAlive { get; }

        float EdgeOffset { get; }
        float Speed { get; }
        float EntryAngleJitterDeg { get; }
    }
}