using _Project.Runtime.Constants;
using _Project.Runtime.Movement;
using _Project.Runtime.Services;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "AsteroidInstaller", menuName = "Installers/Asteroid Installer")]
    public class AsteroidInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            var configs = Container.Resolve<IConfigsService>();

            Container
                .Bind<MovementConfig>()
                .FromInstance(configs.Get<MovementConfig>(AddressablesConfigPaths.Movement.Asteroid))
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InertialMotor>()
                .AsTransient();
        }
    }
}
