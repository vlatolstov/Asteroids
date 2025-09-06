using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct UfoSpawnCommand : IEventData
    {
        public readonly Sprite Sprite;
        public readonly float Scale;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;
        public readonly float AngleRad;

        public UfoSpawnCommand(Sprite sprite, float scale, Vector2 pos, Vector2 vel,
            float angleRad)
        {
            Sprite = sprite;
            Scale = scale;
            Pos = pos;
            Vel = vel;
            AngleRad = angleRad;
        }
    }

    public readonly struct UfoDespawnCommand : IEventData
    {
        public readonly uint ViewId;

        public UfoDespawnCommand(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct UfoViewOffscreen : IEventData
    {
        public readonly uint ViewId;

        public UfoViewOffscreen(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct UfoSpawned : IStateData
    {
        public readonly uint ViewId;
        public readonly Vector2 Position;

        public UfoSpawned(uint viewId, Vector2 position)
        {
            ViewId = viewId;
            Position = position;
        }
    }
    
    public readonly struct UfoDestroyed : IEventData
    {
        public readonly uint ViewId;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;

        public UfoDestroyed(uint viewId, Vector2 pos, Vector2 vel)
        {
            ViewId = viewId;
            Pos = pos;
            Vel = vel;
        }
    }
    
    
}