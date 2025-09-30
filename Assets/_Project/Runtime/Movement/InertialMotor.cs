using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;
using UnityEngine;

namespace _Project.Runtime.Movement
{
    public class InertialMotor : BaseMotor2D
    {
        public InertialMotor(MovementConfig config, IWorldConfig world) : base(config, world)
        {
            SetThrust(1f);
        }

        protected override void ApplyRotation(Rigidbody2D rb, float angleRad)
        {
            //clear override - don't apply rotation over time
        }
    }
}