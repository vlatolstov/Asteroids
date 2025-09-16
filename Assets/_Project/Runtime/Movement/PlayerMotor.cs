using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;

namespace _Project.Runtime.Movement
{
    public class PlayerMotor : BaseMotor2D<IMovementConfig>
    {
        public PlayerMotor(IMovementConfig config, IWorldConfig world) : base(config, world)
        { }
        
    }
}