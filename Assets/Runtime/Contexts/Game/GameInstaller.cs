using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Contexts.Asteroids;
using Runtime.Contexts.Ship;
using Runtime.Models;
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
            ShipLogicBindings();
            AsteroidLogicBindings();
            ViewsContainerBindings();

            Container
                .Bind<IWorldConfig>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IModel>()
                .To<GameModel>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ProjectileHitResolver>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<HudPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<InputPresenter>()
                .AsSingle();
        }


        private void ViewsContainerBindings()
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
        }

        private void ShipLogicBindings()
        {
            Container.BindMemoryPool<ShipView, ShipView.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(_shipPrefab);

            Container
                .BindInterfacesAndSelfTo<ShipPresenter>()
                .AsSingle();
        }

        private void AsteroidLogicBindings()
        {
            Container.BindMemoryPool<AsteroidView, AsteroidView.Pool>()
                .WithInitialSize(50)
                .FromComponentInNewPrefab(_asteroidPrefab)
                .UnderTransformGroup("Asteroids")
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<AsteroidPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AsteroidsLifecycleSystem>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AsteroidSpawnSystem>()
                .AsSingle();
        }
    }
}