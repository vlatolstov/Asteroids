using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Analytics;
using _Project.Runtime.Analytics.Firebase;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.AssetManagement.Configs;
using _Project.Runtime.Asteroid;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Presenters;
using _Project.Runtime.Score;
using _Project.Runtime.Services;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
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
            Container.BindInterfacesAndSelfTo<SceneAssetProvider>().AsSingle();

            Container.Bind<IConfigLoader>().To<AsteroidsSpawnConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<UfoSpawnConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<GeneralSoundsConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<GeneralVisualsConfigLoader>().AsSingle();

            Container.Bind<IConfigLoader>().To<ShipGunConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<ShipLaserConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<UfoBlasterConfigLoader>().AsSingle();

            Container.Bind<IConfigLoader>().To<BlasterPulseConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<RocketConfigLoader>().AsSingle();
            Container.Bind<IConfigLoader>().To<LaserAttackConfigLoader>().AsSingle();
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
                .BindInterfacesAndSelfTo<GameAudioPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AnimationPresenter>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<StatisticsPresenter>()
                .AsSingle();
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
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<GameConfigsService>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<BestScoreService>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<GameLoadingTasksProcessor>()
                .AsSingle();
            
            Container
                .Bind<IAnalyticsLogger>()
                .To<FirebaseAnalyticsLogger>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AnalyticsEventHandler>()
                .AsSingle();
        }
    }
}
