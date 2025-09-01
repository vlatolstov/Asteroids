using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Contexts.Asteroids;
using Runtime.Contexts.Ship;
using Runtime.Models;
using Runtime.Movement;
using Runtime.Settings;
using Runtime.Spawn;
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
        private GameObject _smallAsteroidPrefab;

        [SerializeField]
        private GameObject _largeAsteroidPrefab;

        [SerializeField]
        private bool _autoSpawnShip = true;

        public override void InstallBindings()
        {
            ShipBindings();
            AsteroidBindings();

            Container
                .Bind<IWorldConfig>()
                .To<CameraWorldConfig>()
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

            if (_autoSpawnShip)
            {
                Container
                    .BindInterfacesTo<PlayerShipSpawner>()
                    .AsSingle();
            }
        }

        private void AsteroidBindings()
        {
            Container.BindMemoryPool<AsteroidLargeView, AsteroidLargeView.Pool>()
                .WithInitialSize(15)
                .FromComponentInNewPrefab(_largeAsteroidPrefab)
                .UnderTransformGroup("Asteroids")
                .NonLazy();

            Container.BindMemoryPool<AsteroidSmallView, AsteroidSmallView.Pool>()
                .WithInitialSize(50)
                .FromComponentInNewPrefab(_smallAsteroidPrefab)
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