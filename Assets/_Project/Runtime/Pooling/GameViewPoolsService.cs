using System;
using System.Collections.Generic;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Pooling
{
    public interface IViewPoolsService
    {
        bool IsInitialized { get; }
        event Action Initialized;
        TPool GetPool<TPool>() where TPool : class;
        UniTask LoadPoolsAsync();
    }

    public sealed class GameViewPoolsService : IDisposable, IViewPoolsService
    {
        private readonly DiContainer _container;
        private readonly ViewsContainer _viewsContainer;

        private readonly ShipViewProvider _shipViewProvider;
        private readonly UfoViewProvider _ufoViewProvider;
        private readonly AsteroidViewProvider _asteroidViewProvider;
        private readonly ProjectileViewProvider _projectileViewProvider;
        private readonly AoeAttackViewProvider _aoeAttackViewProvider;
        private readonly AudioSourceViewProvider _audioSourceViewProvider;
        private readonly AnimationViewProvider _animationViewProvider;

        private readonly Dictionary<Type, object> _pools;
        private Transform _root;
        private UniTaskCompletionSource _loadingTcs;

        public bool IsInitialized { get; private set; }
        public event Action Initialized;

        private const int ShipPoolSize = 2;
        private const int UfoPoolSize = 20;
        private const int AsteroidPoolSize = 80;
        private const int ProjectilePoolSize = 100;
        private const int AoePoolSize = 10;
        private const int AudioPoolSize = 100;
        private const int AnimationPoolSize = 20;

        private static readonly string ShipGroup = "Ship";
        private static readonly string UfoGroup = "Ufo's";
        private static readonly string AsteroidGroup = "Asteroids";
        private static readonly string ProjectileGroup = "Projectiles";
        private static readonly string AoeGroup = "AoeViewsPool";
        private static readonly string AudioGroup = "Sound";
        private static readonly string AnimationGroup = "Animations";

        public GameViewPoolsService(
            DiContainer container,
            ViewsContainer viewsContainer,
            ShipViewProvider shipViewProvider,
            UfoViewProvider ufoViewProvider,
            AsteroidViewProvider asteroidViewProvider,
            ProjectileViewProvider projectileViewProvider,
            AoeAttackViewProvider aoeAttackViewProvider,
            AudioSourceViewProvider audioSourceViewProvider,
            AnimationViewProvider animationViewProvider)
        {
            _container = container;
            _viewsContainer = viewsContainer;
            _shipViewProvider = shipViewProvider;
            _ufoViewProvider = ufoViewProvider;
            _asteroidViewProvider = asteroidViewProvider;
            _projectileViewProvider = projectileViewProvider;
            _aoeAttackViewProvider = aoeAttackViewProvider;
            _audioSourceViewProvider = audioSourceViewProvider;
            _animationViewProvider = animationViewProvider;

            _pools = new Dictionary<Type, object>();
        }

        public UniTask LoadPoolsAsync()
        {
            if (IsInitialized)
            {
                return UniTask.CompletedTask;
            }

            if (_loadingTcs != null)
            {
                return _loadingTcs.Task;
            }

            _loadingTcs = new UniTaskCompletionSource();
            LoadInternalAsync().Forget();
            return _loadingTcs.Task;
        }

        private async UniTaskVoid LoadInternalAsync()
        {
            try
            {
                if (!_root)
                {
                    _root = new GameObject("[ViewPools]").transform;
                }

                var shipPrefab = await _shipViewProvider.LoadPrefabAsync();
                var ufoPrefab = await _ufoViewProvider.LoadPrefabAsync();
                var asteroidPrefab = await _asteroidViewProvider.LoadPrefabAsync();
                var projectilePrefab = await _projectileViewProvider.LoadPrefabAsync();
                var aoePrefab = await _aoeAttackViewProvider.LoadPrefabAsync();
                var audioPrefab = await _audioSourceViewProvider.LoadPrefabAsync();
                var animationPrefab = await _animationViewProvider.LoadPrefabAsync();

                RegisterPool(CreateShipPool(shipPrefab));
                RegisterPool(CreateUfoPool(ufoPrefab));
                RegisterPool(CreateAsteroidPool(asteroidPrefab));
                RegisterPool(CreateProjectilePool(projectilePrefab));
                RegisterPool(CreateAoePool(aoePrefab));
                RegisterPool(CreateAudioPool(audioPrefab));
                RegisterPool(CreateAnimationPool(animationPrefab));

                IsInitialized = true;
                Initialized?.Invoke();
                _loadingTcs?.TrySetResult();
                _loadingTcs = null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize view pools: {ex}");
                _loadingTcs?.TrySetException(ex);
                _loadingTcs = null;
            }
        }

        public void Dispose()
        {
            if (_root)
            {
                UnityEngine.Object.Destroy(_root.gameObject);
                _root = null;
            }

            _pools.Clear();
            IsInitialized = false;
            _loadingTcs = null;
        }

        public TPool GetPool<TPool>() where TPool : class
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Pools have not been initialized yet.");
            }

            if (_pools.TryGetValue(typeof(TPool), out var pool))
            {
                return (TPool)pool;
            }

            throw new InvalidOperationException($"Pool of type {typeof(TPool).Name} is not registered.");
        }

        private void RegisterPool<TPool>(TPool pool) where TPool : class
        {
            _pools[typeof(TPool)] = pool;
        }

        private ShipView.Pool CreateShipPool(GameObject prefab)
        {
            var parent = CreateGroup(ShipGroup);
            return new ShipView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<ShipView>(prefab, parent),
                parent, ShipPoolSize);
        }

        private UfoView.Pool CreateUfoPool(GameObject prefab)
        {
            var parent = CreateGroup(UfoGroup);
            return new UfoView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<UfoView>(prefab, parent),
                parent, UfoPoolSize);
        }

        private AsteroidView.Pool CreateAsteroidPool(GameObject prefab)
        {
            var parent = CreateGroup(AsteroidGroup);
            return new AsteroidView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AsteroidView>(prefab, parent),
                parent, AsteroidPoolSize);
        }

        private ProjectileView.Pool CreateProjectilePool(GameObject prefab)
        {
            var parent = CreateGroup(ProjectileGroup);
            return new ProjectileView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<ProjectileView>(prefab, parent),
                parent, ProjectilePoolSize);
        }

        private AoeAttackView.Pool CreateAoePool(GameObject prefab)
        {
            var parent = CreateGroup(AoeGroup);
            return new AoeAttackView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AoeAttackView>(prefab, parent),
                parent, AoePoolSize);
        }

        private AudioSourceView.Pool CreateAudioPool(GameObject prefab)
        {
            var parent = CreateGroup(AudioGroup);
            return new AudioSourceView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AudioSourceView>(prefab, parent),
                parent, AudioPoolSize);
        }

        private AnimationView.Pool CreateAnimationPool(GameObject prefab)
        {
            var parent = CreateGroup(AnimationGroup);
            return new AnimationView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AnimationView>(prefab, parent),
                parent, AnimationPoolSize);
        }

        private Transform CreateGroup(string groupName)
        {
            var group = new GameObject(string.IsNullOrEmpty(groupName) ? "Pool" : groupName);
            group.transform.SetParent(_root, false);
            return group.transform;
        }
    }
}
