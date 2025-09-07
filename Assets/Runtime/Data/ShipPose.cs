using System;
using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct ShipPose : IStateData, IEquatable<ShipPose>
    {
        public readonly Vector2 Position;
        public readonly Vector2 Velocity;
        public readonly float AngleRadians;

        public ShipPose(Vector2 position, Vector2 velocity, float angleRadians)
        {
            Position = position;
            Velocity = velocity;
            AngleRadians = angleRadians;
        }

        public bool Equals(ShipPose other)
        {
            return Position.Equals(other.Position) && Velocity.Equals(other.Velocity) && AngleRadians.Equals(other.AngleRadians);
        }

        public override bool Equals(object obj)
        {
            return obj is ShipPose other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Velocity, AngleRadians);
        }

        public static bool operator ==(ShipPose left, ShipPose right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShipPose left, ShipPose right)
        {
            return !left.Equals(right);
        }
    }
}