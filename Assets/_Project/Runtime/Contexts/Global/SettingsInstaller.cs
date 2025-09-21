using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Settings;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Contexts.Global
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/Settings Installer")]
    public class SettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private AsteroidsSpawnConfig _asteroidsSpawnConfig;
        
        [SerializeField]
        private UfoSpawnConfig _ufoSpawnConfig;

        [SerializeField]
        private ScoreConfig _scoreConfig;
        
        [SerializeField]
        private GeneralSoundsConfig _generalSoundsConfig;
        
        [SerializeField]
        private GeneralVisualsConfig _generalVisualsConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<ScoreConfig>()
                .FromInstance(_scoreConfig)
                .AsSingle();

            Container
                .Bind<AsteroidsSpawnConfig>()
                .FromInstance(_asteroidsSpawnConfig)
                .AsSingle();
            
            Container
                .Bind<UfoSpawnConfig>()
                .FromInstance(_ufoSpawnConfig)
                .AsSingle();
            
            Container
                .Bind<GeneralSoundsConfig>()
                .FromInstance(_generalSoundsConfig)
                .AsSingle();
            
            Container
                .Bind<GeneralVisualsConfig>()
                .FromInstance(_generalVisualsConfig)
                .AsSingle();
        }
    }
}