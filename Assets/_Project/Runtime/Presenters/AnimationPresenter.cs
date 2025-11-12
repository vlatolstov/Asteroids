using System;
using System.Collections.Generic;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Pooling;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
using _Project.Runtime.Views;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class AnimationPresenter : IInitializable, IDisposable
    {
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;
        private readonly AsteroidsModel _asteroidsModel;
        private readonly UfoModel _ufoModel;

        private readonly IViewPoolsService _poolsService;
        private readonly GeneralVisualsConfig _config;

        private readonly Dictionary<uint, AnimationView> _activeViews;
        private AnimationView.Pool _pool;
        private bool _subscriptionsActive;

        public AnimationPresenter(CombatModel combatModel, ShipModel shipModel,
            AsteroidsModel asteroidsModel, UfoModel ufoModel,
            GeneralVisualsConfig config, IViewPoolsService poolsService)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _config = config;
            _poolsService = poolsService;

            _activeViews = new Dictionary<uint, AnimationView>();
        }

        public void Initialize()
        {
            if (_poolsService.IsInitialized)
            {
                OnPoolsInitialized();
            }
            else
            {
                _poolsService.Initialized += OnPoolsInitialized;
            }
        }

        public void Dispose()
        {
            _poolsService.Initialized -= OnPoolsInitialized;

            if (_subscriptionsActive)
            {
                _combatModel.ProjectileHit -= OnProjectileHit;
                _shipModel.ShipDestroyed -= OnShipDestroyed;
                _ufoModel.UfoDestroyed -= OnUfoDestroyed;
                _asteroidsModel.AsteroidDestroyed -= OnAsteroidDestroyed;
                _subscriptionsActive = false;
            }
        }

        private void OnPoolsInitialized()
        {
            _poolsService.Initialized -= OnPoolsInitialized;
            _pool = _poolsService.GetPool<AnimationView.Pool>();

            if (_subscriptionsActive)
            {
                return;
            }

            _combatModel.ProjectileHit += OnProjectileHit;
            _shipModel.ShipDestroyed += OnShipDestroyed;
            _ufoModel.UfoDestroyed += OnUfoDestroyed;
            _asteroidsModel.AsteroidDestroyed += OnAsteroidDestroyed;
            _subscriptionsActive = true;
        }

        private void OnProjectileHit(ProjectileHit hit)
        {
            if (!hit.Projectile.HitAnimation)
            {
                return;
            }

            if (_pool == null)
            {
                return;
            }

            var view = _pool.Spawn(hit.Projectile.HitAnimation, hit.Position, hit.Rotation, hit.Projectile.Size);
            RegisterView(view);
        }

        private void OnShipDestroyed(ShipDestroyed dest)
        {
            if (_pool == null)
            {
                return;
            }

            var view = _pool.Spawn(_config.ShipDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnUfoDestroyed(UfoDestroyed dest)
        {
            if (_pool == null)
            {
                return;
            }

            var view = _pool.Spawn(_config.UfoDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed dest)
        {
            if (_pool == null)
            {
                return;
            }

            var view = _pool.Spawn(_config.AsteroidDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnViewExpired(uint viewId)
        {
            if (!_activeViews.TryGetValue(viewId, out var view))
            {
                throw new Exception("View has not been registered");
            }

            UnregisterView(view);
            _pool?.Despawn(view);
        }

        private void RegisterView(AnimationView v)
        {
            if (!_activeViews.TryAdd(v.ViewId, v))
            {
                throw new Exception("View has already been registered");
            }

            v.Expired += OnViewExpired;
        }

        private void UnregisterView(AnimationView v)
        {
            _activeViews.Remove(v.ViewId);
            v.Expired -= OnViewExpired;
        }
    }
}
