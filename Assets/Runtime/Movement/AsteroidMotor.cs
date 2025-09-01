using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using UnityEngine;

namespace Runtime.Movement
{
    public class AsteroidMotor : BaseMotor2D<IMovementConfig>
    {
        private void OnEnable()
        {
            SetControls(1f, 0f);
        }
    }
}