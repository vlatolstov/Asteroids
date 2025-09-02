using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Movement;
using Runtime.Settings;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Asteroids
{
    [CreateAssetMenu(fileName = "AsteroidInstaller", menuName = "Installers/Asteroid Installer")]
    public class AsteroidInstaller : ScriptableObjectInstaller
    {
        public MovementConfig MovementConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<IMovementConfig>()
                .FromInstance(MovementConfig)
                .AsSingle();

            Container
                .Bind<BaseMotor2D<IMovementConfig>>()
                .To<AsteroidMotor>()
                .AsTransient();
        }
    }
}