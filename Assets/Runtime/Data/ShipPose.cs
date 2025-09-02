using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Data
{
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
}