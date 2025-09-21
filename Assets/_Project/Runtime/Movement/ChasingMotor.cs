using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Data;
using _Project.Runtime.Settings;
using UnityEngine;
using GM = _Project.Runtime.Utils.GeometryMethods;

namespace _Project.Runtime.Movement
{
    public class ChasingMotor : BaseMotor2D<MovementConfig>
    {
        private readonly ChasingEnemyConfig _chase;
        
        private float _prevErr;
        private ShipPose _target;

        public ChasingMotor(MovementConfig config, IWorldConfig world, ChasingEnemyConfig chase) : base(config, world)
        {
            _chase = chase;
        }

        protected override void UpdateControls(float dt)
        {
            var fwd = GM.AngleToDir(AngleRadians);

            Vector2 deltaMove;
            if (Config.IsWrappedByWorldBounds)
            {
                deltaMove = GM.ShortestWrappedDelta(Position, _target.Position, World.WorldRect);
            }
            else
            {
                deltaMove = _target.Position - Position;
            }
            
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