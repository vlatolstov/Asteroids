
using UnityEngine;

namespace _Project.Runtime.Data
{
    public readonly struct UfoSpawnCommand
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


    public readonly struct UfoOffscreen 
    {
        public readonly uint ViewId;

        public UfoOffscreen(uint viewId)
        {
            ViewId = viewId;
        }
    }

    public readonly struct UfoSpawned 
    {
        public readonly uint ViewId;
        public readonly Vector2 Position;

        public UfoSpawned(uint viewId, Vector2 position)
        {
            ViewId = viewId;
            Position = position;
        }
    }
    
    public readonly struct UfoDestroyed
    {
        public readonly uint ViewId;
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;

        public UfoDestroyed(uint viewId, Vector2 position, Quaternion rotation, Vector2 scale )
        {
            ViewId = viewId;
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }
    }
    
    
}