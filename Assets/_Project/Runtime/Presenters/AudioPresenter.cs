using System;
using System.Collections.Generic;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Pooling;
using _Project.Runtime.Services;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class AudioPresenter : IInitializable, IDisposable
    {
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;

        private readonly IViewPoolsService _poolsService;
        private readonly IConfigsService _configsService;

        private readonly Dictionary<uint, AudioSourceView> _activeViews;
        private AudioSourceView.Pool _audioPool;
        private GeneralSoundsConfig _generalSounds;
        private bool _subscriptionsActive;
        private bool _poolsReady;
        private bool _configsReady;

        public AudioPresenter(CombatModel combatModel, ShipModel shipModel,
            IViewPoolsService poolsService, IConfigsService configsService)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _poolsService = poolsService;
            _configsService = configsService;

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

            UniTask.Void(LoadConfigAsync);
        }

        private async UniTaskVoid LoadConfigAsync()
        {
            await _configsService.LoadAllAsync();
            _generalSounds = _configsService.Get<GeneralSoundsConfig>(AddressablesConfigPaths.General.GeneralSounds);
            _configsReady = true;
            TrySubscribe();
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
            _poolsReady = true;
            TrySubscribe();
        }

        private void TrySubscribe()
        {
            if (_subscriptionsActive || !_poolsReady || !_configsReady)
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
