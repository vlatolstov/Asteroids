using UnityEngine;

namespace _Project.Runtime.Data
{
    public enum AsteroidSize
    {
        Large,
        Small
    }

    public readonly struct AsteroidSpawnCommand
    {
        public readonly Sprite Sprite;
        public readonly AsteroidSize Size;
        public readonly float Scale;
        public readonly Vector2 Pos;
        public readonly Vector2 Vel;
        public readonly float AngleRad;
        public readonly float AngRotation;

        public AsteroidSpawnCommand(Sprite sprite, AsteroidSize size, float scale, Vector2 pos, Vector2 vel,
            float angleRad, float angRotation)
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

    public readonly struct AsteroidDespawnCommand
    {
        public readonly uint ViewId;

        public AsteroidDespawnCommand(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct AsteroidOffscreen
    {
        public readonly uint ViewId;
        public readonly AsteroidSize Size;

        public AsteroidOffscreen(uint viewId, AsteroidSize size)
        {
            ViewId = viewId;
            Size = size;
        }
    }

    public readonly struct AsteroidDestroyed
    {
        public readonly uint ViewId;
        public readonly AsteroidSize Size;
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;
        public readonly Vector2 Velocity;

        public AsteroidDestroyed(uint viewId, AsteroidSize size, Vector2 position,
            Quaternion rotation, Vector2 scale, Vector2 velocity)
        {
            ViewId = viewId;
            Size = size;
            Position = position;
            Velocity = velocity;
            Scale = scale;
            Rotation = rotation;
        }
    }
}