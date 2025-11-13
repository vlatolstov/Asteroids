using _Project.Runtime.Constants;
using _Project.Runtime.Movement;
using _Project.Runtime.Services;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "ShipInstaller", menuName = "Installers/Ship Installer")]
    public class ShipInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            var configs = Container.Resolve<IConfigsService>();

            Container
                .Bind<MovementConfig>()
                .FromInstance(configs.Get<MovementConfig>(AddressablesConfigPaths.Movement.Ship))
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<PlayerMotor>()
                .AsTransient();

            Container
                .Bind<ProjectileWeaponConfig>()
                .FromInstance(configs.Get<ProjectileWeaponConfig>(AddressablesConfigPaths.Weapons.ShipGun))
                .AsSingle();
            
            Container
                .Bind<AoeWeaponConfig>()
                .FromInstance(configs.Get<AoeWeaponConfig>(AddressablesConfigPaths.Weapons.ShipLaser))
                .AsSingle();
        }
    }
}
