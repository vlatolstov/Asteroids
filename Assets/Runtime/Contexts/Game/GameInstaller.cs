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
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField]
        private GameObject _shipPrefab;

        [SerializeField]
        private GameObject _asteroidPrefab;

        public override void InstallBindings()
        {
            ShipBindings();
            AsteroidBindings();

            Container
                .Bind<IWorldConfig>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<BaseView>()
                .FromComponentsInHierarchy()
                .AsSingle()
                .WhenInjectedInto<IViewsContainer>();

            Container
                .Bind<IViewsContainer>()
                .To<ViewsContainer>()
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
        }


        private void ShipBindings()
        {
            Container
                .BindInterfacesAndSelfTo<ShipPresenter>()
                .AsSingle();

            Container.BindMemoryPool<ShipView, ShipView.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(_shipPrefab);
        }

        private void AsteroidBindings()
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