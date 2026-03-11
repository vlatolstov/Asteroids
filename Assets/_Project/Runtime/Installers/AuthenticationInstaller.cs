using _Project.Runtime.AssetManagement;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Presenters;
using Zenject;

namespace _Project.Runtime.Installers
{
    public class AuthenticationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindLoadingTasks();
            BindServices();
            BindModels();
            BindPresenters();
        }

        private void BindLoadingTasks()
        {
            Container
                .BindInterfacesAndSelfTo<AuthenticationLoadingTasksProcessor>()
                .AsSingle();
        }

        private void BindServices()
        {
            Container
                .BindInterfacesAndSelfTo<SceneAssetProvider>()
                .AsSingle();
        }

        private void BindModels()
        {
            Container
                .BindInterfacesAndSelfTo<AuthenticationModel>()
                .AsSingle();
        }

        private void BindPresenters()
        {
            Container
                .BindInterfacesTo<AuthenticationPresenter>()
                .AsSingle();
        }
    }
}
