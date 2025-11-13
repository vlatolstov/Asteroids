using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Analytics;
using _Project.Runtime.Analytics.Firebase;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Asteroid;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Pooling;
using _Project.Runtime.Presenters;
using _Project.Runtime.Score;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/Game Installer")]
    public class GameInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            AssetProvidersBindings();
            ViewsBindings();
            PresentersBindings();
            ModelsBindings();
            ServicesBindings();

            Container
                .Bind<IWorldConfig>()
                .FromComponentInHierarchy()
                .AsSingle();
        }

        private void AssetProvidersBindings()
        {
            Container
                .BindInterfacesAndSelfTo<ShipViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<UfoViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<AsteroidViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<ProjectileViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<AoeAttackViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<AnimationViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<AudioSourceViewProvider>()
                .AsSingle();
            
            
            Container
                .BindInterfacesAndSelfTo<BackgroundViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<BGMViewProvider>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<HudViewProvider>()
                .AsSingle();
        }

        private void ViewsBindings()
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

        private void PresentersBindings()
        {
            Container
                .BindInterfacesAndSelfTo<HudPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InputPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ShipPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<UfoPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AsteroidsPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<CombatPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<BackgroundPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<GameStatePresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ScorePresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AudioPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AnimationPresenter>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<StatisticsPresenter>()
                .AsSingle()
                .NonLazy();
        }

        private void ModelsBindings()
        {
            Container
                .BindInterfacesAndSelfTo<GameModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InputModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ShipModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<UfoModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AsteroidsModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ScoreModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<CombatModel>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<StatisticsModel>()
                .AsSingle();
        }

        private void ServicesBindings()
        {
            Container
                .BindInterfacesAndSelfTo<GameViewPoolsService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<BestScoreService>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<GameLoadingTaskService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<IAnalyticsLogger>()
                .To<FirebaseAnalyticsLogger>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AnalyticsEventHandler>()
                .AsSingle()
                .NonLazy();
        }
    }
}
