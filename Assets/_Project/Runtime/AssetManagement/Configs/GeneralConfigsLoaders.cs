using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Constants;
using _Project.Runtime.Settings;
using _Project.Runtime.Ufo;

namespace _Project.Runtime.AssetManagement.Configs
{
    public class AsteroidsSpawnConfigLoader : LocalConfigLoader<AsteroidsSpawnConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.General.AsteroidsSpawn;
    }

    public class UfoSpawnConfigLoader : LocalConfigLoader<UfoSpawnConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.General.UfoSpawn;
    }

    public class ScoreConfigLoader : LocalConfigLoader<ScoreConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.General.Score;
    }

    public class GeneralSoundsConfigLoader : LocalConfigLoader<GeneralSoundsConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.General.GeneralSounds;
    }

    public class GeneralVisualsConfigLoader : LocalConfigLoader<GeneralVisualsConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.General.GeneralVisuals;
    }

    public class BackgroundJitterConfigLoader : LocalConfigLoader<BackgroundJitterConfig>
    {
        protected override string AssetPath => AddressablesConfigPaths.General.BackgroundJitter;
    }
}
