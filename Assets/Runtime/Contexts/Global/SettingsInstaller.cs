using Runtime.Abstract.Configs;
using Runtime.Settings;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Global
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Settings/Settings Installer")]
    public class SettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private ShipConfig _shipConfig;

        [SerializeField]
        private WorldConfig _worldConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<ShipConfig>()
                .FromInstance(_shipConfig)
                .AsSingle();
            
            Container.Bind<IWorldConfig>()
                .FromInstance(_worldConfig)
                .AsSingle();
        }
    }
}