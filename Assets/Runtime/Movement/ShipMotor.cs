using Runtime.Abstract.Movement;
using Runtime.Data;
using Runtime.Settings;
using UnityEngine;

namespace Runtime.Movement
{
    public class ShipMotor : BaseMotor2D<ShipConfig>
    {
        protected override (float thrust, float turnAxis) ReadControlAxes()
        {
            float thrust = 0f, turnAxis = 0f;
            if (Model.TryGet<ThrustInput>(out var thrustInput))
            {
                thrust = thrustInput.Value;
            }

            if (Model.TryGet<TurnInput>(out var turnInput))
            {
                turnAxis = turnInput.Value;
            }

            return (thrust, turnAxis);
        }

        protected override bool TryReadPoseFromModel(out Vector2 pos, out Vector2 vel, out float angleRad)
        {
            if (Model.TryGet<ShipPose>(out var pose))
            {
                pos = pose.Position;
                vel = pose.Velocity;
                angleRad = pose.AngleRadians;
                return true;
            }

            pos = default;
            vel = default;
            angleRad = 0f;
            return false;
        }

        protected override void WritePoseToModel(Vector2 pos, Vector2 vel, float angleRad)
        {
            Model.ChangeData<ShipPose>(_ => new ShipPose(pos, vel, angleRad));
        }
    }
}