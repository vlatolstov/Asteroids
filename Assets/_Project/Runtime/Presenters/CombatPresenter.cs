using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Views;

namespace _Project.Runtime.Presenters
{
    public class CombatPresenter : IDisposable
    {
        private readonly CombatModel _combatModel;

        private readonly ProjectileView.Pool _projectilePool;
        private readonly AoeAttackView.Pool _aoePool;

        private readonly Dictionary<uint, ProjectileView> _activeProjectiles;
        private readonly Dictionary<uint, AoeAttackView> _activeAoe;

        public CombatPresenter(CombatModel combatModel, ProjectileView.Pool projectilePool, AoeAttackView.Pool aoePool)
        {
            _combatModel = combatModel;

            _projectilePool = projectilePool;
            _aoePool = aoePool;

            _activeProjectiles = new Dictionary<uint, ProjectileView>();
            _activeAoe = new Dictionary<uint, AoeAttackView>();

            _combatModel.ProjectileShot += OnProjectileShot;
            _combatModel.AoeAttackReleased += OnAoeAttackReleased;
        }

        public void Dispose()
        {
            _combatModel.ProjectileShot -= OnProjectileShot;
            _combatModel.AoeAttackReleased -= OnAoeAttackReleased;
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
            
            _projectilePool.Despawn(projView);
            _activeProjectiles.Remove(viewId);
        }


        private void SpawnAoe(AoeAttackReleased aoe)
        {
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
            
            _aoePool.Despawn(aoeView);
            _activeAoe.Remove(viewId);
        }
    }
}