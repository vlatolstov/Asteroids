using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public enum AsteroidSize
    {
        Large,
        Small
    }

    public readonly struct AsteroidId
    {
        public readonly int Value;

        public AsteroidId(int v)
        {
            Value = v;
        }

        public override string ToString() => Value.ToString();
    }

    public readonly struct AsteroidViewOffscreen : IData
    {
        public readonly AsteroidId Id;
        public readonly AsteroidSize Size;

        public AsteroidViewOffscreen(AsteroidId id, AsteroidSize size)
        {
            Id = id;
            Size = size;
        }
    }

    public readonly struct AsteroidViewDestroyed : IData
    {
        public readonly AsteroidId Id;
        public readonly AsteroidSize Size;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;

        public AsteroidViewDestroyed(AsteroidId id, AsteroidSize size, Vector2 pos, Vector2 vel)
        {
            Id = id;
            Size = size;
            Pos = pos;
            Vel = vel;
        }
    }

    
    public readonly struct AsteroidSpawnRequest : IData
    {
        public readonly AsteroidId Id;
        public readonly AsteroidSize Size;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;
        public readonly float AngleRad;

        public AsteroidSpawnRequest(AsteroidId id, AsteroidSize size, Vector2 pos, Vector2 vel, float angleRad)
        {
            Id = id;
            Size = size;
            Pos = pos;
            Vel = vel;
            AngleRad = angleRad;
        }
    }

    public readonly struct AsteroidDespawnRequest : IData
    {
        public readonly AsteroidId Id;

        public AsteroidDespawnRequest(AsteroidId id)
        {
            Id = id;
        }
    }
}