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

    public struct ScoreAdded : IData
    {
        public int Amount;

        public ScoreAdded(int amount)
        {
            Amount = amount;
        }
    }

    public struct ProjectileHit : IData
    {
        public Faction Source;
        public GameObject Target;
        public Vector2 Point;

        public ProjectileHit(Faction source, GameObject target, Vector2 point)
        {
            Source = source;
            Target = target;
            Point = point;
        }
    }
}