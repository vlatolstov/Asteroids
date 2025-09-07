using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;

namespace Runtime.Movement
{
    public class PlayerMotor : BaseMotor2D<IMovementConfig>
    {
        public PlayerMotor(IMovementConfig config, IWorldConfig world) : base(config, world)
        { }
        
    }
}