using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.Ads;
using _Project.Runtime.LoadingServices;
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