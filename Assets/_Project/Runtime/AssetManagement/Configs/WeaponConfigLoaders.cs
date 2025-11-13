using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.Weapons;

namespace _Project.Runtime.AssetManagement.Configs
{
    public class ShipGunConfigLoader : LocalConfigLoader<ProjectileWeaponConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Weapons.ShipGun;
    }

    public class ShipLaserConfigLoader : LocalConfigLoader<AoeWeaponConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Weapons.ShipLaser;
    }

    public class UfoBlasterConfigLoader : LocalConfigLoader<ProjectileWeaponConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.Weapons.UfoBlaster;
    }
}
