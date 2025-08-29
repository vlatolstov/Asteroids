using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Models;
using Runtime.Movement;
using Runtime.Settings;
using Runtime.Views;
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
                .BindInterfacesAndSelfTo<ShipPresenter>()
                .AsSingle();

            Container.BindMemoryPool<ShipView, ShipView.Pool>()
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