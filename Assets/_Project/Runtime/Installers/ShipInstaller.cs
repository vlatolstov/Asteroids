using _Project.Runtime.Movement;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "ShipInstaller", menuName = "Installers/Ship Installer")]
    public class ShipInstaller : ScriptableObjectInstaller
    {
        public MovementConfig MovementConfig;
        public ProjectileWeaponConfig ShipGun;
        public AoeWeaponConfig AoeWeapon;

        public override void InstallBindings()
        {
            Container
                .Bind<MovementConfig>()
                .FromInstance(MovementConfig)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<PlayerMotor>()
                .AsTransient();

            Container
                .BindInterfacesAndSelfTo<ProjectileWeaponConfig>()
                .FromInstance(ShipGun)
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<AoeWeaponConfig>()
                .FromInstance(AoeWeapon)
                .AsSingle();
        }
    }
}