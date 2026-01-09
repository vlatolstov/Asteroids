using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.Weapons;

namespace _Project.Runtime.AssetManagement.ResourceLoaders
{
    public class ShipGunResourceLoader : LocalResourceLoader<ProjectileWeaponResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Weapons.ShipGun;
    }

    public class ShipLaserResourceLoader : LocalResourceLoader<AoeWeaponResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Weapons.ShipLaser;
    }

    public class UfoBlasterResourceLoader : LocalResourceLoader<ProjectileWeaponResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.Weapons.UfoBlaster;
    }
}
