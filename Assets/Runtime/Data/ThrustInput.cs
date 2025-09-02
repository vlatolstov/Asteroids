using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public struct ThrustInput : IStateData
    {
        public float Value;

        public ThrustInput(float value)
        {
            Value = value;
        }
    }

    public struct TurnInput : IStateData
    {
        public float Value;

        public TurnInput(float value)
        {
            Value = value;
        }
    }

    public struct FireLaserPressed : IEventData
    { }

    public struct FireBulletPressed : IEventData
    { }

    public struct BulletState : IStateData
    {
        public float Cooldown;

        public BulletState(float cooldown)
        {
            Cooldown = cooldown;
        }
    }

    public struct LaserState : IStateData
    {
        public int Charges;
        public float Cooldown;

        public LaserState(int charges, float cooldown)
        {
            Charges = charges;
            Cooldown = cooldown;
        }
    }

    public struct ScoreAdded : IEventData
    {
        public int Amount;

        public ScoreAdded(int amount)
        {
            Amount = amount;
        }
    }

    public struct TotalScore : IStateData
    {
        public int Amount;
    }

    public struct ProjectileHit : IEventData
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