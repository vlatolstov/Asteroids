using _Project.Runtime.Movement;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "AsteroidInstaller", menuName = "Installers/Asteroid Installer")]
    public class AsteroidInstaller : ScriptableObjectInstaller
    {
        public MovementConfig MovementConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<MovementConfig>()
                .FromInstance(MovementConfig)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InertialMotor>()
                .AsTransient();
        }
    }
}