using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.Movement;
using _Project.Runtime.Settings;

namespace _Project.Runtime.AssetManagement.Configs
{
    public class ShipMovementConfigLoader : LocalConfigLoader<MovementConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Movement.Ship;
    }

    public class AsteroidMovementConfigLoader : LocalConfigLoader<MovementConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Movement.Asteroid;
    }

    public class UfoMovementConfigLoader : LocalConfigLoader<MovementConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Movement.Ufo;
    }

    public class UfoChasingConfigLoader : LocalConfigLoader<ChasingEnemyConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Movement.UfoChasing;
    }
}
