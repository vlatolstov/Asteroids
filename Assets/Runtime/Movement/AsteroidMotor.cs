using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using UnityEngine;

namespace Runtime.Movement
{
    public class AsteroidMotor : BaseMotor2D<IMovementConfig>
    {
        public AsteroidMotor(IMovementConfig config, IWorldConfig world) : base(config, world)
        {
            SetControls(1f, 0f);
        }
    }
}