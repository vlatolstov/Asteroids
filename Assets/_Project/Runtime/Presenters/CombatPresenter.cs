using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Pooling;
using _Project.Runtime.Views;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class CombatPresenter : IInitializable, IDisposable
    {
        private readonly CombatModel _combatModel;

        private readonly IViewPoolsService _poolsService;

        private readonly Dictionary<uint, ProjectileView> _activeProjectiles;
        private readonly Dictionary<uint, AoeAttackView> _activeAoe;
        private ProjectileView.Pool _projectilePool;
        private AoeAttackView.Pool _aoePool;
        private bool _subscriptionsActive;

        public CombatPresenter(CombatModel combatModel, IViewPoolsService poolsService)
        {
            _combatModel = combatModel;

            _poolsService = poolsService;

            _activeProjectiles = new Dictionary<uint, ProjectileView>();
            _activeAoe = new Dictionary<uint, AoeAttackView>();
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
                _combatModel.ProjectileShot -= OnProjectileShot;
                _combatModel.AoeAttackReleased -= OnAoeAttackReleased;
                _subscriptionsActive = false;
            }
        }

        private void OnPoolsInitialized()
        {
            _poolsService.Initialized -= OnPoolsInitialized;
            _projectilePool = _poolsService.GetPool<ProjectileView.Pool>();
            _aoePool = _poolsService.GetPool<AoeAttackView.Pool>();

            if (_subscriptionsActive)
            {
                return;
            }

            _combatModel.ProjectileShot += OnProjectileShot;
            _combatModel.AoeAttackReleased += OnAoeAttackReleased;
            _subscriptionsActive = true;
        }

        private void OnProjectileShot(ProjectileShot shot)
        {
            SpawnProjectile(shot);
        }

        private void OnAoeAttackReleased(AoeAttackReleased aoe)
        {
            SpawnAoe(aoe);
        }

        private void OnProjectileHit(uint viewId, ProjectileHit hit)
        {
            DespawnProjectile(viewId);
            _combatModel.HandleProjectileHit(hit);
        }

        private void OnAoeAttackHit(AoeHit hit)
        {
            _combatModel.HandleAoeHit(hit);
        }

        private void OnProjectileExpired(uint viewId)
        {
            DespawnProjectile(viewId);
        }

        private void OnAoeAttackExpired(uint viewId)
        {
            DespawnAoe(viewId);
        }

        private void SpawnProjectile(ProjectileShot shot)
        {
            if (_projectilePool == null)
            {
                return;
            }

            var proj = _projectilePool.Spawn(shot);

            if (!_activeProjectiles.TryAdd(proj.ViewId, proj))
            {
                _projectilePool.Despawn(proj);
                throw new AggregateException($"Projectile with {proj.ViewId} already registered");
            }

            proj.ProjectileHit += OnProjectileHit;
            proj.Expired += OnProjectileExpired;
        }

        private void DespawnProjectile(uint viewId)
        {
            if (!_activeProjectiles.TryGetValue(viewId, out var projView))
            {
                throw new AggregateException($"Projectile with {viewId} not registered");
            }

            projView.ProjectileHit -= OnProjectileHit;
            projView.Expired -= OnProjectileExpired;

            _projectilePool?.Despawn(projView);
            _activeProjectiles.Remove(viewId);
        }


        private void SpawnAoe(AoeAttackReleased aoe)
        {
            if (_aoePool == null)
            {
                return;
            }

            var attack = _aoePool.Spawn(aoe.Emitter, aoe.Weapon, aoe.Source);

            if (!_activeAoe.TryAdd(attack.ViewId, attack))
            {
                _aoePool.Despawn(attack);
                throw new AggregateException($"Aoe attack with {attack.ViewId} already registered");
            }

            attack.AoeHit += OnAoeAttackHit;
            attack.Expired += OnAoeAttackExpired;
        }

        private void DespawnAoe(uint viewId)
        {
            if (!_activeAoe.TryGetValue(viewId, out var aoeView))
            {
                throw new AggregateException($"Aoe view with {viewId} not registered");
            }

            aoeView.AoeHit -= OnAoeAttackHit;
            aoeView.Expired -= OnAoeAttackExpired;

            _aoePool?.Despawn(aoeView);
            _activeAoe.Remove(viewId);
        }
    }
}
