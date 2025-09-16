using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Movement;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Contexts.Ship
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
                .Bind<IMovementConfig>()
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