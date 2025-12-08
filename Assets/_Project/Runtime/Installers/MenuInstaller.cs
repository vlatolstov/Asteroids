using _Project.Runtime.AssetManagement;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Presenters;
using _Project.Runtime.Score;
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
                .BindInterfacesAndSelfTo<MenuLoadingTasksProcessor>()
                .AsSingle();
        }
    }
}