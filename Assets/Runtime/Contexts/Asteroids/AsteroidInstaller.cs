using Runtime.Abstract.Configs;
using Runtime.Movement;
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
                .BindInterfacesAndSelfTo<InertialMotor>()
                .AsTransient();
        }
    }
}