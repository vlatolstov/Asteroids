using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Analytics;
using _Project.Runtime.Analytics.Firebase;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.AssetManagement.ResourceLoaders;
using _Project.Runtime.Asteroid;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Presenters;
using _Project.Runtime.Score;
using _Project.Runtime.Services;
using _Project.Runtime.Settings;
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
                .To<CameraWorldBounds>()
                .AsSingle()
                .NonLazy();
        }

        private void AssetProvidersBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneAssetProvider>().AsSingle();

            Container.Bind<IResourceLoader>().To<AsteroidsSpawnResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<UfoSpawnResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<GeneralSoundsResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<GeneralVisualsResourceLoader>().AsSingle();

            Container.Bind<IResourceLoader>().To<ShipGunResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<ShipLaserResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<ShipPowerShieldResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<UfoBlasterResourceLoader>().AsSingle();

            Container.Bind<IResourceLoader>().To<BlasterPulseResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<RocketResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<LaserAttackResourceLoader>().AsSingle();
            Container.Bind<IResourceLoader>().To<PowerShieldAttackResourceLoader>().AsSingle();
        }

        private void PresentersBindings()
        {
            Container
                .BindInterfacesTo<HudPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<InputPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<ShipPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<UfoPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<AsteroidsPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<CombatPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<BackgroundPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<GameStatePresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<ScorePresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<GameAudioPresenter>()
                .AsSingle();

            Container
                .BindInterfacesTo<AnimationPresenter>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<StatisticsPresenter>()
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
                .BindInterfacesTo<GameViewPoolsService>()
                .AsSingle();

            Container
                .BindInterfacesTo<GameResourcesService>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<GameLoadingTasksProcessor>()
                .AsSingle();
            
            Container
                .Bind<IAnalyticsLogger>()
                .To<FirebaseAnalyticsLogger>()
                .AsSingle();

            Container
                .BindInterfacesTo<AnalyticsEventHandler>()
                .AsSingle();
        }
    }
}
