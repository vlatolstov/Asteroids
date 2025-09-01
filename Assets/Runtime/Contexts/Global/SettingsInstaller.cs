using Runtime.Abstract.Configs;
using Runtime.Movement;
using Runtime.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Runtime.Contexts.Global
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Settings/Settings Installer")]
    public class SettingsInstaller : ScriptableObjectInstaller
    {
        [Header("Movement")]
        [SerializeField]
        private MovementConfig _shipConfig;

        [SerializeField]
        private MovementConfig _asteroidConfig;

        [SerializeField]
        private MovementConfig _ufoConfig;

        [Header("Spawn")]
        [SerializeField]
        private AsteroidsSpawnConfig _asteroidsSpawnConfig;
        [Header("Score")]
        [SerializeField]
        private ScoreConfig _scoreConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<IMovementConfig>()
                .FromInstance(_asteroidConfig)
                .WhenInjectedInto<AsteroidMotor>();

            Container
                .Bind<IMovementConfig>()
                .FromInstance(_shipConfig)
                .WhenInjectedInto<ShipMotor>();

            Container
                .Bind<IMovementConfig>()
                .FromInstance(_ufoConfig)
                .WhenInjectedInto<UfoMotor>();

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