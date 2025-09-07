using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.Weapons;
using Runtime.Movement;
using Runtime.Settings;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Ship
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
                .Bind<BaseMotor2D<IMovementConfig>>()
                .To<ShipMotor>()
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