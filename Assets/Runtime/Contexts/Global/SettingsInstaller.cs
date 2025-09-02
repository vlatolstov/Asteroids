using Runtime.Abstract.Configs;
using Runtime.Movement;
using Runtime.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Runtime.Contexts.Global
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/Settings Installer")]
    public class SettingsInstaller : ScriptableObjectInstaller
    {
        [Header("Spawn")]
        [SerializeField]
        private AsteroidsSpawnConfig _asteroidsSpawnConfig;

        [Header("Score")]
        [SerializeField]
        private ScoreConfig _scoreConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<IScoreConfig>()
                .FromInstance(_scoreConfig)
                .AsSingle();

            Container
                .Bind<IAsteroidsSpawnConfig>()
                .FromInstance(_asteroidsSpawnConfig)
                .AsSingle();
        }
    }
}