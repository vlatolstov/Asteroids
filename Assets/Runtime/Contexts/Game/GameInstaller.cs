using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Contexts.Asteroids;
using Runtime.Contexts.Ship;
using Runtime.Models;
using Runtime.Utils;
using Runtime.Views;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Game
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/Game Installer")]
    public class GameInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private GameObject _shipPrefab;

        [SerializeField]
        private GameObject _asteroidPrefab;

        public override void InstallBindings()
        {
            ViewsBindings();
            PresentersBindings();
            ModelsBindings();
            DeclareSignals();

            Container
                .Bind<IWorldConfig>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ProjectileHitResolver>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<AsteroidsLifecycleSystem>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AsteroidSpawnSystem>()
                .AsSingle();
        }

        private void ViewsBindings()
        {
            Container
                .Bind<BaseView>()
                .FromComponentsInHierarchy()
                .AsSingle()
                .WhenInjectedInto<IViewsContainer>();

            Container
                .Bind<IViewsContainer>()
                .To<ViewsContainer>()
                .AsSingle();
            
            Container.BindMemoryPool<ShipView, ShipView.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(_shipPrefab);
            
            Container.BindMemoryPool<AsteroidView, AsteroidView.Pool>()
                .WithInitialSize(50)
                .FromComponentInNewPrefab(_asteroidPrefab)
                .UnderTransformGroup("Asteroids")
                .NonLazy();
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
                .BindInterfacesAndSelfTo<AsteroidPresenter>()
                .AsSingle();
        }

        private void ModelsBindings()
        {
            Container
                .Bind<IModel>()
                .To<GameModel>()
                .AsSingle();
        }


        private void DeclareSignals()
        {
            SignalBusInstaller.Install(Container);
            
            var signalTypes = TypeHelpers.GetTypesImplementingInterface<IData>();

            foreach (var type in signalTypes)
            {
                Container.DeclareSignal(type);
            }
        }
    }
}