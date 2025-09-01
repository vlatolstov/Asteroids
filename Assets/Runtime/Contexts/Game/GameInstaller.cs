using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Models;
using Runtime.Movement;
using Runtime.Settings;
using Runtime.Views;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject _shipPrefab;

        [SerializeField]
        private bool _autoSpawnShip = true;

        public override void InstallBindings()
        {
            ShipBindings();
            
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
    }
}