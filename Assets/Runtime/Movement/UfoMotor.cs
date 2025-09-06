using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Settings;
using UnityEngine;
using UnityEngine.LightTransport;
using GM = Runtime.Utils.GeometryMethods;

namespace Runtime.Movement
{
    public class UfoMotor : BaseMotor2D<IMovementConfig>
    {
        public UfoMotor(IMovementConfig config, IWorldConfig world) : base(config, world)
        { }

        private float _prevErr;

        public void ChaseShip(ShipPose target,
            ChasingEnemyConfig chase,
            IWorldConfig world,
            Rigidbody2D thisRb)
        {
            // float dt = Time.fixedDeltaTime;
            //
            // Vector2 selfPos = Position;
            // Vector2 fwd = GM.AngleToDir(AngleRadians);
            //
            // Vector2 delta = GM.ShortestWrappedDelta(selfPos, target.Position, world.WorldRect);
            // float dist = delta.magnitude;
            // Vector2 aimDir = dist > 1e-5f ? delta / dist : fwd;
            //
            // float angErr = GM.SignedAngleRad(fwd, aimDir);
            // float dErr = (angErr - _prevErr) / Mathf.Max(dt, 1e-5f);
            // _prevErr = angErr;
            //
            // float turnAxis = Mathf.Clamp(chase.TurnKp * angErr + chase.TurnKd * dErr, -1f, 1f);
            //
            // float thrust = Mathf.Clamp(chase.ThrustKp * dist, 0f, chase.MaxThrust);
            //
            // float aimFactor = Mathf.Clamp01(1f - Mathf.Abs(angErr) / (45f * Mathf.Deg2Rad));
            // thrust *= Mathf.Lerp(0.35f, 1f, aimFactor);
            //
            // SetThrust(thrust);
            // SetTurnAxis(turnAxis);
        }
    }
}