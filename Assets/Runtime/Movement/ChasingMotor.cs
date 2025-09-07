using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using UnityEngine;
using GM = Runtime.Utils.GeometryMethods;

namespace Runtime.Movement
{
    public class ChasingMotor : BaseMotor2D<IMovementConfig>
    {
        private float _prevErr;
        private ShipPose _target;
        private IChasingEnemyConfig _chase;

        public ChasingMotor(IMovementConfig config, IWorldConfig world, IChasingEnemyConfig chase) : base(config, world)
        {
            _chase = chase;
        }

        protected override void UpdateControls(float dt)
        {
            var fwd = GM.AngleToDir(AngleRadians);
            var deltaMove = GM.ShortestWrappedDelta(Position, _target.Position, World.WorldRect);
            float distMove = deltaMove.magnitude;
            var aimDir = distMove > 1e-5f ? deltaMove / distMove : fwd;

            float angErr = GM.SignedAngleRad(fwd, aimDir);
            float dErr = (angErr - _prevErr) / Mathf.Max(dt, 1e-5f);
            _prevErr = angErr;

            float turnAxis = Mathf.Clamp(-(_chase.TurnKp * angErr + _chase.TurnKd * dErr), -1f, 1f);

            float thrust = Mathf.Clamp(_chase.ThrustKp * distMove, 0f, _chase.MaxThrust);
            float aimFactor = Mathf.Clamp01(1f - Mathf.Abs(angErr) / (45f * Mathf.Deg2Rad));
            thrust *= Mathf.Lerp(0.35f, 1f, aimFactor);

            SetThrust(thrust);
            SetTurnAxis(turnAxis);
        }

        public void ChaseTarget(ShipPose target)
        {
            _target = target;
        }
    }
}