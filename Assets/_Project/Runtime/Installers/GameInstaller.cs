using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Analytics;
using _Project.Runtime.Analytics.Firebase;
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

        [SerializeField, Min(0)]
        private int _shipPoolSize = 2;

        [SerializeField, Min(0)]
        private int _ufoPoolSize = 20;

        [SerializeField, Min(0)]
        private int _asteroidPoolSize = 80;

        [SerializeField, Min(0)]
        private int _projectilePoolSize = 100;

        [SerializeField, Min(0)]
        private int _aoePoolSize = 10;

        [SerializeField, Min(0)]
        private int _audioPoolSize = 100;

        [SerializeField, Min(0)]
        private int _animationPoolSize = 20;

        private const string ShipPoolGroup = "Ship";
        private const string UfoPoolGroup = "Ufo's";
        private const string AsteroidPoolGroup = "Asteroids";
        private const string ProjectilePoolGroup = "Projectiles";
        private const string AoePoolGroup = "AoeViewsPool";
        private const string AudioPoolGroup = "Sound";
        private const string AnimationPoolGroup = "Animations";

        public override void InstallBindings()
        {
            ViewsBindings();
            PresentersBindings();
            ModelsBindings();
            ServicesBindings();

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
                .WhenInjectedInto<ViewsContainer>();

            Container
                .Bind<ViewsContainer>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ViewPoolsService>()
                .AsSingle()
                .WithArguments(CreatePoolSettings())
                .NonLazy();
        }

        private ViewPoolsService.Settings CreatePoolSettings()
        {
            return new ViewPoolsService.Settings
            {
                Ship = CreateConfig(_shipPrefab, _shipPoolSize, ShipPoolGroup),
                Ufo = CreateConfig(_ufoPrefab, _ufoPoolSize, UfoPoolGroup),
                Asteroid = CreateConfig(_asteroidPrefab, _asteroidPoolSize, AsteroidPoolGroup),
                Projectile = CreateConfig(_projectilePrefab, _projectilePoolSize, ProjectilePoolGroup),
                Aoe = CreateConfig(_aoeAttackPrefab, _aoePoolSize, AoePoolGroup),
                Audio = CreateConfig(_audioSourcePrefab, _audioPoolSize, AudioPoolGroup),
                Animation = CreateConfig(_animationPrefab, _animationPoolSize, AnimationPoolGroup)
            };
        }

        private static ViewPoolsService.PoolConfig CreateConfig(GameObject prefab, int size, string groupName)
        {
            return new ViewPoolsService.PoolConfig
            {
                Prefab = prefab,
                InitialSize = Mathf.Max(0, size),
                ParentGroup = groupName
            };
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
