using _Project.Runtime.AssetManagement;
using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.AssetManagement.ResourceLoaders;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Presenters;
using _Project.Runtime.Services;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "MenuInstaller", menuName = "Installers/MenuInstaller")]
    public class MenuInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            BindLoadingTasks();
            BindServices();
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
                .BindInterfacesAndSelfTo<GameResourcesService>()
                .AsSingle();
        }

        private void BindPresenters()
        {
            Container
                .BindInterfacesAndSelfTo<MenuPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ShopPresenter>()
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
