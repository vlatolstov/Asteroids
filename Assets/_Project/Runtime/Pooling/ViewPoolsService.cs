using System;
using System.Collections.Generic;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Pooling
{
    public interface IViewPoolsService
    {
        bool IsInitialized { get; }
        event Action Initialized;
        TPool GetPool<TPool>() where TPool : class;
    }

    public sealed class ViewPoolsService : IInitializable, IDisposable, IViewPoolsService
    {
        public class PoolConfig
        {
            public GameObject Prefab;
            public int InitialSize;
            public string ParentGroup;
        }

        public class Settings
        {
            public PoolConfig Ship;
            public PoolConfig Ufo;
            public PoolConfig Asteroid;
            public PoolConfig Projectile;
            public PoolConfig Aoe;
            public PoolConfig Audio;
            public PoolConfig Animation;
        }

        private readonly DiContainer _container;
        private readonly ViewsContainer _viewsContainer;
        private readonly Settings _settings;

        private readonly Dictionary<Type, object> _pools;
        private Transform _root;

        public bool IsInitialized { get; private set; }
        public event Action Initialized;

        public ViewPoolsService(DiContainer container, ViewsContainer viewsContainer, Settings settings)
        {
            _container = container;
            _viewsContainer = viewsContainer;
            _settings = settings;

            _pools = new Dictionary<Type, object>();
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            _root = new GameObject("[ViewPools]").transform;

            RegisterPool(CreateShipPool());
            RegisterPool(CreateUfoPool());
            RegisterPool(CreateAsteroidPool());
            RegisterPool(CreateProjectilePool());
            RegisterPool(CreateAoePool());
            RegisterPool(CreateAudioPool());
            RegisterPool(CreateAnimationPool());

            IsInitialized = true;
            Initialized?.Invoke();
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

        private ShipView.Pool CreateShipPool()
        {
            var config = _settings.Ship;
            var parent = CreateGroup(config);
            return new ShipView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<ShipView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private UfoView.Pool CreateUfoPool()
        {
            var config = _settings.Ufo;
            var parent = CreateGroup(config);
            return new UfoView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<UfoView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private AsteroidView.Pool CreateAsteroidPool()
        {
            var config = _settings.Asteroid;
            var parent = CreateGroup(config);
            return new AsteroidView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AsteroidView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private ProjectileView.Pool CreateProjectilePool()
        {
            var config = _settings.Projectile;
            var parent = CreateGroup(config);
            return new ProjectileView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<ProjectileView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private AoeAttackView.Pool CreateAoePool()
        {
            var config = _settings.Aoe;
            var parent = CreateGroup(config);
            return new AoeAttackView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AoeAttackView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private AudioSourceView.Pool CreateAudioPool()
        {
            var config = _settings.Audio;
            var parent = CreateGroup(config);
            return new AudioSourceView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AudioSourceView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private AnimationView.Pool CreateAnimationPool()
        {
            var config = _settings.Animation;
            var parent = CreateGroup(config);
            return new AnimationView.Pool(_viewsContainer, () =>
                _container.InstantiatePrefabForComponent<AnimationView>(config.Prefab, parent),
                parent, config.InitialSize);
        }

        private Transform CreateGroup(PoolConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (!config.Prefab)
            {
                throw new ArgumentException($"Prefab for group '{config.ParentGroup}' is not set.", nameof(config));
            }

            var group = new GameObject(string.IsNullOrEmpty(config.ParentGroup) ? "Pool" : config.ParentGroup);
            group.transform.SetParent(_root, false);
            return group.transform;
        }
    }
}
