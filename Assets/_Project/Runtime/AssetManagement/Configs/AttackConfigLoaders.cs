using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.Weapons;

namespace _Project.Runtime.AssetManagement.Configs
{
    public class BlasterPulseConfigLoader : LocalConfigLoader<ProjectileConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Attacks.BlasterPulse;
    }

    public class RocketConfigLoader : LocalConfigLoader<ProjectileConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Attacks.Rocket;
    }

    public class LaserAttackConfigLoader : LocalConfigLoader<AoeAttackConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Attacks.LaserAttack;
    }
}
