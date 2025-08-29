using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public struct ThrustInput : IData
    {
        public float Value;
        
        public ThrustInput(float value)
        {
            Value = value;
        }
    }

    public struct TurnInput : IData
    {
        public float Value;

        public TurnInput(float value)
        {
            Value = value;
        }
    }

    public struct FireLaserPressed : IData
    { }

    public struct FireBulletPressed : IData
    { }

    public struct ShipPose : IData
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float AngleRadians;

        public ShipPose(Vector2 position, Vector2 velocity, float angleRadians)
        {
            Position = position;
            Velocity = velocity;
            AngleRadians = angleRadians;
        }
    }

    public struct BulletState : IData
    {
        public float Cooldown;

        public BulletState(float cooldown)
        {
            Cooldown = cooldown;
        }
    }

    public struct LaserState : IData
    {
        public int Charges;
        public float Cooldown;

        public LaserState(int charges, float cooldown)
        {
            Charges = charges;
            Cooldown = cooldown;
        }
    }

    public struct ShipSpawned : IData
    {
        public Vector2 Position;

        public ShipSpawned(Vector2 position)
        {
            Position = position;
        }
    }

    public struct ShipDestroyed : IData
    { }
}