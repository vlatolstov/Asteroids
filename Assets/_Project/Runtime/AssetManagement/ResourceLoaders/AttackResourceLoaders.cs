using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.Weapons;

namespace _Project.Runtime.AssetManagement.ResourceLoaders
{
    public class BlasterPulseResourceLoader : LocalResourceLoader<ProjectileResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Attacks.BlasterPulse;
    }

    public class RocketResourceLoader : LocalResourceLoader<ProjectileResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Attacks.Rocket;
    }

    public class LaserAttackResourceLoader : LocalResourceLoader<AoeAttackResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Attacks.LaserAttack;
    }

    public class PowerShieldAttackResourceLoader : LocalResourceLoader<AoeAttackResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Attacks.PowerShieldAttack;
    }
}
