using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Pooling;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class AudioPresenter : IInitializable, IDisposable
    {
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;

        private readonly IViewPoolsService _poolsService;
        private readonly GeneralSoundsConfig _generalSounds;

        private readonly Dictionary<uint, AudioSourceView> _activeViews;
        private AudioSourceView.Pool _audioPool;
        private bool _subscriptionsActive;

        public AudioPresenter(CombatModel combatModel, ShipModel shipModel,
            GeneralSoundsConfig generalSounds, IViewPoolsService poolsService)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _generalSounds = generalSounds;
            _poolsService = poolsService;

            _activeViews = new Dictionary<uint, AudioSourceView>();
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
                _combatModel.ProjectileHit -= OnProjectileHit;
                _combatModel.AoeAttackReleased -= OnAoeAttackReleased;
                _combatModel.AoeHit -= OnAoeHit;
                _shipModel.ShipSpawned -= OnShipSpawned;
                _subscriptionsActive = false;
            }
        }

        private void OnPoolsInitialized()
        {
            _poolsService.Initialized -= OnPoolsInitialized;
            _audioPool = _poolsService.GetPool<AudioSourceView.Pool>();

            if (_subscriptionsActive)
            {
                return;
            }

            _combatModel.ProjectileShot += OnProjectileShot;
            _combatModel.ProjectileHit += OnProjectileHit;
            _combatModel.AoeAttackReleased += OnAoeAttackReleased;
            _combatModel.AoeHit += OnAoeHit;

            _shipModel.ShipSpawned += OnShipSpawned;
            _subscriptionsActive = true;
        }

        private void OnProjectileShot(ProjectileShot shot)
        {
            SpawnAndRegister(shot.Position, shot.Weapon.AttackSound);
        }

        private void OnProjectileHit(ProjectileHit hit)
        {
            SpawnAndRegister(hit.Position, hit.Projectile.HitSound);
        }

        private void OnAoeAttackReleased(AoeAttackReleased attack)
        {
            SpawnAndRegister(attack.Emitter.position, attack.Weapon.AttackSound);
        }

        private void OnAoeHit(AoeHit hit)
        {
            SpawnAndRegister(hit.Position, hit.Attack.HitSound);
        }

        private void OnShipSpawned(ShipSpawned spawned)
        {
            SpawnAndRegister(spawned.Position, _generalSounds.ShipSpawn);
        }

        private void OnViewExpired(uint viewId)
        {
            if (!_activeViews.TryGetValue(viewId, out var view))
            {
                throw new Exception("View has not been registered");
            }

            UnregisterView(view);
            _audioPool?.Despawn(view);
        }

        private void SpawnAndRegister(Vector2 position, AudioClip clip)
        {
            if (_audioPool == null)
            {
                return;
            }

            var view = _audioPool.Spawn(position, clip);
            RegisterView(view);
        }

        private void RegisterView(AudioSourceView v)
        {
            if (!_activeViews.TryAdd(v.ViewId, v))
            {
                throw new Exception("View has already been registered");
            }

            v.Expired += OnViewExpired;
        }

        private void UnregisterView(AudioSourceView v)
        {
            _activeViews.Remove(v.ViewId);
            v.Expired -= OnViewExpired;
        }
    }
}
