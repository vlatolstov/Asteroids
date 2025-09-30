using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Data;
using _Project.Runtime.Settings;
using UnityEngine;
using GM = _Project.Runtime.Utils.GeometryMethods;

namespace _Project.Runtime.Movement
{
    public class ChasingMotor : BaseMotor2D
    {
        private readonly ChasingEnemyConfig _chaseConfig;
        
        private float _previousError;
        private ShipPose _target;

        public ChasingMotor(MovementConfig config, IWorldConfig world, 
            ChasingEnemyConfig chaseConfig) : base(config, world)
        {
            _chaseConfig = chaseConfig;
        }
        
        public void ChaseTarget(ShipPose target)
        {
            _target = target;
        }

        protected override void UpdateControls(float dt)
        {
            var forward = GM.AngleToDir(AngleRadians);

            var deltaMove = Config.IsWrappedByWorldBounds
                ? GM.ShortestWrappedDelta(Position, _target.Position, World.WorldRect)
                : _target.Position - Position;
            
            float distanceToTarget = deltaMove.magnitude;
            var aimDirection = distanceToTarget > 1e-5f ? deltaMove / distanceToTarget : forward;

            float angleError = GM.SignedAngleRad(forward, aimDirection);
            float angleErrorDelta = (angleError - _previousError) / Mathf.Max(dt, 1e-5f);
            
            _previousError = angleError;

            float turnAxis = Mathf.Clamp(-(_chaseConfig.TurnKp * angleError + _chaseConfig.TurnKd * angleErrorDelta), -1f, 1f);
            float thrust = Mathf.Clamp(_chaseConfig.ThrustKp * distanceToTarget, 0f, _chaseConfig.MaxThrust);
            
            float aimFactor = Mathf.Clamp01(1f - Mathf.Abs(angleError) / (45f * Mathf.Deg2Rad));
            
            thrust *= Mathf.Lerp(0.35f, 1f, aimFactor);

            SetThrust(thrust);
            SetTurnAxis(turnAxis);
        }
    }
}