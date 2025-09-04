using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct ShipPose : IStateData
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
    }
}