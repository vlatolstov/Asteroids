using _Project.Runtime.AssetManagement;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Presenters;
using _Project.Runtime.Score;
using _Project.Runtime.Views;
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
                .Bind<ShopView>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<BestScoreService>()
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
