using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Movement;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Contexts.Asteroids
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