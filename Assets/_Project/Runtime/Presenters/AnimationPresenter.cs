using System;
using System.Collections.Generic;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Services;
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
        private readonly IConfigsService _configsService;

        private readonly Dictionary<uint, AnimationView> _activeViews;
        private AnimationView.Pool _pool;
        private GeneralVisualsConfig _visuals;
        private bool _subscriptionsActive;

        public AnimationPresenter(CombatModel combatModel, ShipModel shipModel,
            AsteroidsModel asteroidsModel, UfoModel ufoModel,
            IViewPoolsService poolsService, IConfigsService configsService)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _poolsService = poolsService;
            _configsService = configsService;

            _activeViews = new Dictionary<uint, AnimationView>();
        }

        public void Initialize()
        {
            if (_poolsService.IsReady)
            {
                OnPoolsReady();
                return;
            }

            _poolsService.Ready += OnPoolsReady;
        }

        public void Dispose()
        {
            _poolsService.Ready -= OnPoolsReady;

            if (_subscriptionsActive)
            {
                _combatModel.ProjectileHit -= OnProjectileHit;
                _shipModel.ShipDestroyed -= OnShipDestroyed;
                _ufoModel.UfoDestroyed -= OnUfoDestroyed;
                _asteroidsModel.AsteroidDestroyed -= OnAsteroidDestroyed;
                _subscriptionsActive = false;
            }
        }

        private void TrySubscribe()
        {
            if (_subscriptionsActive || _pool == null || _visuals == null)
            {
                return;
            }

            _combatModel.ProjectileHit += OnProjectileHit;
            _shipModel.ShipDestroyed += OnShipDestroyed;
            _ufoModel.UfoDestroyed += OnUfoDestroyed;
            _asteroidsModel.AsteroidDestroyed += OnAsteroidDestroyed;
            _subscriptionsActive = true;
        }

        private void OnPoolsReady()
        {
            _poolsService.Ready -= OnPoolsReady;

            _pool = _poolsService.GetPool<AnimationView.Pool>();
            _visuals = _configsService.Get<GeneralVisualsConfig>(AddressablesConfigPaths.General.GeneralVisuals);

            TrySubscribe();
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

            var view = _pool.Spawn(_visuals.ShipDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnUfoDestroyed(UfoDestroyed dest)
        {
            if (_pool == null)
            {
                return;
            }

            var view = _pool.Spawn(_visuals.UfoDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed dest)
        {
            if (_pool == null)
            {
                return;
            }

            var view = _pool.Spawn(_visuals.AsteroidDestroyed, dest.Position, dest.Rotation, dest.Scale);
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
