
using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;

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