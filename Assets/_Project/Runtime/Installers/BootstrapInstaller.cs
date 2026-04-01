using _Project.Runtime.LoadingServices;
using Zenject;

namespace _Project.Runtime.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<BootstrapLoadingTasksProcessor>()
                .AsCached();
        }
    }
}