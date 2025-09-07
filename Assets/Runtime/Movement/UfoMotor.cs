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
    }
}