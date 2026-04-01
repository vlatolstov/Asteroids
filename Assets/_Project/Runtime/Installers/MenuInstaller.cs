using _Project.Runtime.AssetManagement;
using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.AssetManagement.ResourceLoaders;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Presenters;
using _Project.Runtime.Services;
using Zenject;

namespace _Project.Runtime.Installers
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindLoadingTasks();
            BindServices();
            BindModels();
            BindPresenters();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<SceneAssetProvider>().AsSingle();

            Container
                .Bind<IResourceLoader>()
                .To<ShopVisualCatalogResourceLoader>()
                .AsSingle();

            Container
                .BindInterfacesTo<GameResourcesService>()
                .AsSingle();
        }

        private void BindPresenters()
        {
            Container
                .BindInterfacesTo<MenuPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ShopPresenter>()
                .AsSingle();
        }

        private void BindModels()
        {
            Container
                .BindInterfacesAndSelfTo<ShopModel>()
                .AsSingle();
        }

        private void BindLoadingTasks()
        {
            Container
                .BindInterfacesAndSelfTo<MenuLoadingTasksProcessor>()
                .AsSingle();
        }
    }
}
