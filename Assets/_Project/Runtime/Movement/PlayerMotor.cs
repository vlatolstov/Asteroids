using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.RemoteConfig;

namespace _Project.Runtime.Movement
{
    public class PlayerMotor : BaseMotor2D
    {
        public PlayerMotor(MovementConfigData config, IWorldConfig world) : base(config, world)
        { }
    }
}
