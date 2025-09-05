using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public enum AsteroidSize
    {
        Large,
        Small
    }
    
    public readonly struct AsteroidSpawnRequest : IEventData
    {
        public readonly Sprite Sprite;
        public readonly AsteroidSize Size;
        public readonly float Scale;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;
        public readonly float AngleRad;
        public readonly float AngRotation;

        public AsteroidSpawnRequest(Sprite sprite, AsteroidSize size, float scale, Vector2 pos, Vector2 vel, float angleRad, float angRotation)
        {
            Sprite = sprite;
            Size = size;
            Scale = scale;
            Pos = pos;
            Vel = vel;
            AngleRad = angleRad;
            AngRotation = angRotation;
        }
    }

    public readonly struct AsteroidDespawnRequest : IEventData
    {
        public readonly uint ViewId;

        public AsteroidDespawnRequest(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct AsteroidViewOffscreen : IEventData
    {
        public readonly uint ViewId;
        public readonly AsteroidSize Size;

        public AsteroidViewOffscreen(uint viewId, AsteroidSize size)
        {
            ViewId = viewId;
            Size = size;
        }
    }

    public readonly struct AsteroidDestroyed : IEventData
    {
        public readonly uint ViewId;
        public readonly AsteroidSize Size;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;

        public AsteroidDestroyed(uint viewId, AsteroidSize size, Vector2 pos, Vector2 vel)
        {
            ViewId = viewId;
            Size = size;
            Pos = pos;
            Vel = vel;
        }
    }
}