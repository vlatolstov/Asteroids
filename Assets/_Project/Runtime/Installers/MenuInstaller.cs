using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Presenters;
using _Project.Runtime.Score;
using _Project.Runtime.Services;
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
            BindViews();
            BindServices();
            BindPresenters();
            BindLoadingTasks();
        }

        private void BindViews()
        {
            Container
                .Bind<BaseView>()
                .FromComponentsInHierarchy()
                .AsSingle()
                .WhenInjectedInto<ViewsContainer>();

            Container
                .Bind<ViewsContainer>()
                .AsSingle();
        }

        private void BindServices()
        {
            Container
                .BindInterfacesAndSelfTo<BestScoreService>()
                .AsSingle();
        }

        private void BindPresenters()
        {
            Container
                .BindInterfacesAndSelfTo<MenuPresenter>()
                .AsSingle();
        }

        private void BindLoadingTasks()
        {
            Container
                .BindInterfacesAndSelfTo<MenuLoadingTaskService>()
                .AsSingle()
                .NonLazy();
        }
    }
}
