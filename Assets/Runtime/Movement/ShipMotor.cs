using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;

namespace Runtime.Movement
{
    public class ShipMotor : BaseMotor2D<IMovementConfig>
    {
        public ShipMotor(IMovementConfig config, IWorldConfig world) : base(config, world)
        { }
        
    }
}