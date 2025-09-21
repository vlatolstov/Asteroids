using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;

namespace _Project.Runtime.Movement
{
    public class PlayerMotor : BaseMotor2D<MovementConfig>
    {
        public PlayerMotor(MovementConfig config, IWorldConfig world) : base(config, world)
        { }
    }
}