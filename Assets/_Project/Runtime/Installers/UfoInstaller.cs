using _Project.Runtime.Constants;
using _Project.Runtime.Movement;
using _Project.Runtime.Services;
using _Project.Runtime.Settings;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "UfoInstaller", menuName = "Installers/UFO Installer")]
    public class UfoInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            var configs = Container.Resolve<IConfigsService>();

            Container
                .Bind<MovementConfig>()
                .FromInstance(configs.Get<MovementConfig>(AddressablesConfigPaths.Movement.Ufo))
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ChasingMotor>()
                .AsTransient();

            Container
                .Bind<ProjectileWeaponConfig>()
                .FromInstance(configs.Get<ProjectileWeaponConfig>(AddressablesConfigPaths.Weapons.UfoBlaster))
                .AsSingle();
            
            Container
                .Bind<ChasingEnemyConfig>()
                .FromInstance(configs.Get<ChasingEnemyConfig>(AddressablesConfigPaths.Movement.UfoChasing))
                .AsSingle();
        }
    }
}
