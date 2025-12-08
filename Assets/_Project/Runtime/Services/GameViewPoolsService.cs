using System;
using System.Collections.Generic;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Asteroid;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Services
{
    public interface IViewPoolsService
    {
        TPool GetPool<TPool>() where TPool : class;
    }

    public sealed class GameViewPoolsService : IDisposable, IInitializable, IViewPoolsService
    {
        private readonly DiContainer _container;
        private readonly SceneAssetProvider _assetProvider;
        private readonly GameLoadingTasksProcessor _processor;
        private readonly Dictionary<Type, object> _pools;
        private Transform _root;

        private const int ShipPoolSize = 2;
        private const int UfoPoolSize = 20;
        private const int AsteroidPoolSize = 80;
        private const int ProjectilePoolSize = 100;
        private const int AoePoolSize = 10;
        private const int AudioPoolSize = 100;
        private const int AnimationPoolSize = 20;

        private const string ShipGroup = "Ship";
        private const string UfoGroup = "Ufo's";
        private const string AsteroidGroup = "Asteroids";
        private const string ProjectileGroup = "Projectiles";
        private const string AudioGroup = "Sound";
        private const string AoeGroup = "AoeViewsPool";
        private const string AnimationGroup = "Animations";

        public GameViewPoolsService(
            DiContainer container,
            SceneAssetProvider assetProvider, GameLoadingTasksProcessor processor)
        {
            _container = container;
            _assetProvider = assetProvider;
            _processor = processor;

            _pools = new Dictionary<Type, object>();
        }
        
        public void Initialize()
        {
            _processor.OnTasksFinished += OnTaskProcessorFinished;
        }

        public void Dispose()
        {
            _processor.OnTasksFinished -= OnTaskProcessorFinished;
            
            if (_root)
            {
                UnityEngine.Object.Destroy(_root.gameObject);
                _root = null;
            }

            _pools.Clear();
        }
        
        private void OnTaskProcessorFinished()
        {
            _processor.OnTasksFinished -= OnTaskProcessorFinished;
            LoadPools();
        }

        private void LoadPools()
        {
            try
            {
                if (!_root)
                {
                    _root = new GameObject("[ViewPools]").transform;
                }

                if (_assetProvider.TryGetLoadedComponent(out ShipView shipPrefab))
                {
                    RegisterPool(CreateShipPool(shipPrefab.gameObject));
                }

                if (_assetProvider.TryGetLoadedComponent(out UfoView ufoPrefab))
                {
                    RegisterPool(CreateUfoPool(ufoPrefab.gameObject));
                }

                if (_assetProvider.TryGetLoadedComponent(out AsteroidView asteroidPrefab))
                {
                    RegisterPool(CreateAsteroidPool(asteroidPrefab.gameObject));
                }

                if (_assetProvider.TryGetLoadedComponent(out ProjectileView projectilePrefab))
                {
                    RegisterPool(CreateProjectilePool(projectilePrefab.gameObject));
                }

                if (_assetProvider.TryGetLoadedComponent(out AoeAttackView aoePrefab))
                {
                    RegisterPool(CreateAoePool(aoePrefab.gameObject));
                }

                if (_assetProvider.TryGetLoadedComponent(out AudioSourceView audioPrefab))
                {
                    RegisterPool(CreateAudioPool(audioPrefab.gameObject));
                }

                if (_assetProvider.TryGetLoadedComponent(out AnimationView animationPrefab))
                {
                    RegisterPool(CreateAnimationPool(animationPrefab.gameObject));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize view pools: {ex}");
            }
        }

        public TPool GetPool<TPool>() where TPool : class
        {
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
            return new ShipView.Pool(() =>
                    _container.InstantiatePrefabForComponent<ShipView>(prefab, parent),
                parent, ShipPoolSize);
        }

        private UfoView.Pool CreateUfoPool(GameObject prefab)
        {
            var parent = CreateGroup(UfoGroup);
            return new UfoView.Pool(() =>
                    _container.InstantiatePrefabForComponent<UfoView>(prefab, parent),
                parent, UfoPoolSize);
        }

        private AsteroidView.Pool CreateAsteroidPool(GameObject prefab)
        {
            var parent = CreateGroup(AsteroidGroup);
            return new AsteroidView.Pool(() =>
                    _container.InstantiatePrefabForComponent<AsteroidView>(prefab, parent),
                parent, AsteroidPoolSize);
        }

        private ProjectileView.Pool CreateProjectilePool(GameObject prefab)
        {
            var parent = CreateGroup(ProjectileGroup);
            return new ProjectileView.Pool(() =>
                    _container.InstantiatePrefabForComponent<ProjectileView>(prefab, parent),
                parent, ProjectilePoolSize);
        }

        private AoeAttackView.Pool CreateAoePool(GameObject prefab)
        {
            var parent = CreateGroup(AoeGroup);
            return new AoeAttackView.Pool(() =>
                    _container.InstantiatePrefabForComponent<AoeAttackView>(prefab, parent),
                parent, AoePoolSize);
        }

        private AudioSourceView.Pool CreateAudioPool(GameObject prefab)
        {
            var parent = CreateGroup(AudioGroup);
            return new AudioSourceView.Pool(() =>
                    _container.InstantiatePrefabForComponent<AudioSourceView>(prefab, parent),
                parent, AudioPoolSize);
        }

        private AnimationView.Pool CreateAnimationPool(GameObject prefab)
        {
            var parent = CreateGroup(AnimationGroup);
            return new AnimationView.Pool(() =>
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