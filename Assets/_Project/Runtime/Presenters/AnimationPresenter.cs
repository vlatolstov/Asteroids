using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Settings;
using _Project.Runtime.Views;

namespace _Project.Runtime.Presenters
{
    public class AnimationPresenter : IDisposable
    {
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;
        private readonly AsteroidsModel _asteroidsModel;
        private readonly UfoModel _ufoModel;

        private readonly AnimationView.Pool _pool;
        private readonly GeneralVisualsConfig _config;

        private readonly Dictionary<uint, AnimationView> _activeViews;

        public AnimationPresenter(CombatModel combatModel, ShipModel shipModel,
            AsteroidsModel asteroidsModel, UfoModel ufoModel,
            GeneralVisualsConfig config, AnimationView.Pool pool)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _config = config;
            _pool = pool;
            
            _activeViews = new Dictionary<uint, AnimationView>();

            _combatModel.ProjectileHit += OnProjectileHit;
            _shipModel.ShipDestroyed += OnShipDestroyed;
            _ufoModel.UfoDestroyed += OnUfoDestroyed;
            _asteroidsModel.AsteroidDestroyed += OnAsteroidDestroyed;
        }


        public void Dispose()
        {
            _combatModel.ProjectileHit -= OnProjectileHit;
            _shipModel.ShipDestroyed -= OnShipDestroyed;
            _ufoModel.UfoDestroyed -= OnUfoDestroyed;
            _asteroidsModel.AsteroidDestroyed -= OnAsteroidDestroyed;
        }

        private void OnProjectileHit(ProjectileHit hit)
        {
            if (!hit.Projectile.HitAnimation)
            {
                return;
            }
            var view = _pool.Spawn(hit.Projectile.HitAnimation, hit.Position, hit.Rotation, hit.Projectile.Size);
            RegisterView(view);
        }

        private void OnShipDestroyed(ShipDestroyed dest)
        {
            var view = _pool.Spawn(_config.ShipDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnUfoDestroyed(UfoDestroyed dest)
        {
            var view = _pool.Spawn(_config.UfoDestroyed, dest.Position, dest.Rotation, dest.Scale);
            RegisterView(view);
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed dest)
        {
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
            _pool.Despawn(view);
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