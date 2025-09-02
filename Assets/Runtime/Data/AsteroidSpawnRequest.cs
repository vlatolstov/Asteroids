using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public enum AsteroidSize
    {
        Large,
        Small
    }

    public readonly struct AsteroidViewOffscreen : IData
    {
        public readonly uint ViewId;
        public readonly AsteroidSize Size;

        public AsteroidViewOffscreen(uint viewId, AsteroidSize size)
        {
            ViewId = viewId;
            Size = size;
        }
    }

    public readonly struct AsteroidViewDestroyed : IData
    {
        public readonly uint ViewId;
        public readonly AsteroidSize Size;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;

        public AsteroidViewDestroyed(uint viewId, AsteroidSize size, Vector2 pos, Vector2 vel)
        {
            ViewId = viewId;
            Size = size;
            Pos = pos;
            Vel = vel;
        }
    }

    
    public readonly struct AsteroidSpawnRequest : IData
    {
        public readonly AsteroidSize Size;
        public readonly float Scale;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;
        public readonly float AngleRad;

        public AsteroidSpawnRequest(AsteroidSize size, float scale, Vector2 pos, Vector2 vel, float angleRad)
        {
            Size = size;
            Scale = scale;
            Pos = pos;
            Vel = vel;
            AngleRad = angleRad;
        }
    }

    public readonly struct AsteroidDespawnRequest : IData
    {
        public readonly uint ViewId;

        public AsteroidDespawnRequest(uint viewId)
        {
            ViewId = viewId;
        }
    }
}