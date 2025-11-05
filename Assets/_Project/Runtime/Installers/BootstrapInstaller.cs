using _Project.Runtime.Services;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "BootstrapInstaller", menuName = "Installers/Bootstrap Installer", order = 0)]
    public class BootstrapInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<BootstrapLoadingTaskService>()
                .AsCached()
                .NonLazy();
        }
    }
}