using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Movement;
using Runtime.Settings;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Ufo
{
    [CreateAssetMenu(fileName = "UfoInstaller", menuName = "Installers/UFO Installer")]
    public class UfoInstaller : ScriptableObjectInstaller
    {
        public MovementConfig MovementConfig;
        public ChasingEnemyConfig ChaseConfig;
        public ProjectileWeaponConfig UfoGun;

        public override void InstallBindings()
        {
            Container
                .Bind<IMovementConfig>()
                .FromInstance(MovementConfig)
                .AsSingle();

            Container
                .Bind<BaseMotor2D<IMovementConfig>>()
                .To<UfoMotor>()
                .AsTransient();

            Container
                .BindInterfacesAndSelfTo<ProjectileWeaponConfig>()
                .FromInstance(UfoGun)
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<ChasingEnemyConfig>()
                .FromInstance(ChaseConfig)
                .AsSingle();
        }
    }
}