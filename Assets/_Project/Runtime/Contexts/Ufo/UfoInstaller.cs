using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Movement;
using _Project.Runtime.Settings;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Contexts.Ufo
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
                .Bind<MovementConfig>()
                .FromInstance(MovementConfig)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ChasingMotor>()
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