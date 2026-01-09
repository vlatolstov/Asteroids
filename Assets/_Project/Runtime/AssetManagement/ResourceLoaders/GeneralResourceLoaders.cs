using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Constants;
using _Project.Runtime.Settings;
using _Project.Runtime.Ufo;

namespace _Project.Runtime.AssetManagement.ResourceLoaders
{
    public class AsteroidsSpawnResourceLoader : LocalResourceLoader<AsteroidsSpawnResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.General.AsteroidsSpawn;
    }

    public class UfoSpawnResourceLoader : LocalResourceLoader<UfoSpawnResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.General.UfoSpawn;
    }

    public class GeneralSoundsResourceLoader : LocalResourceLoader<GeneralSoundsResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.General.GeneralSounds;
    }

    public class GeneralVisualsResourceLoader : LocalResourceLoader<GeneralVisualsResource>
    {
        protected override string AssetPath => AddressablesResourcePaths.General.GeneralVisuals;
    }

}
