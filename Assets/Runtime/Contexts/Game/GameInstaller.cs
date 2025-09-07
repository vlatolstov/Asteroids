using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Contexts.Asteroids;
using Runtime.Contexts.Ship;
using Runtime.Models;
using Runtime.Presenters;
using Runtime.Utils;
using Runtime.Views;
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
        private GameObject _ufoPrefab;

        [SerializeField]
        private GameObject _asteroidPrefab;

        [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField]
        private GameObject _aoeAttackPrefab;

        [SerializeField]
        private GameObject _audioSourcePrefab;

        [SerializeField]
        private GameObject _animationPrefab;

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
                .WithInitialSize(2)
                .FromComponentInNewPrefab(_shipPrefab)
                .UnderTransformGroup("Ship")
                .NonLazy();

            Container.BindMemoryPool<UfoView, UfoView.Pool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_ufoPrefab)
                .UnderTransformGroup("Ufo's")
                .NonLazy();

            Container.BindMemoryPool<AsteroidView, AsteroidView.Pool>()
                .WithInitialSize(80)
                .FromComponentInNewPrefab(_asteroidPrefab)
                .UnderTransformGroup("Asteroids")
                .NonLazy();

            Container.BindMemoryPool<ProjectileView, ProjectileView.Pool>()
                .WithInitialSize(100)
                .FromComponentInNewPrefab(_projectilePrefab)
                .UnderTransformGroup("Projectiles")
                .NonLazy();

            Container.BindMemoryPool<AoeAttackView, AoeAttackView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_aoeAttackPrefab)
                .UnderTransformGroup("AoeViewsPool")
                .NonLazy();

            Container.BindMemoryPool<AudioSourceView, AudioSourceView.Pool>()
                .WithInitialSize(100)
                .FromComponentInNewPrefab(_audioSourcePrefab)
                .UnderTransformGroup("Sound")
                .NonLazy();

            Container.BindMemoryPool<AnimationView, AnimationView.Pool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_animationPrefab)
                .UnderTransformGroup("Animations")
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
                .BindInterfacesAndSelfTo<UfoPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<AsteroidsPresenter>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<WeaponPresenter>()
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