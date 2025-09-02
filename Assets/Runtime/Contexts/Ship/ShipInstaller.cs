using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Movement;
using Runtime.Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Runtime.Contexts.Ship
{
    [CreateAssetMenu(fileName = "ShipInstaller", menuName = "Installers/Ship Installer")]
    public class ShipInstaller : ScriptableObjectInstaller
    {
        public MovementConfig MovementConfig;
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
        }
    }
}