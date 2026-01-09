using System;
using System.Collections.Generic;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Models;
using _Project.Runtime.Services;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class GameAudioPresenter : IInitializable, IDisposable
    {
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;

        private readonly IViewPoolsService _poolsService;
        private readonly IResourcesService _resourcesService;
        private readonly GameLoadingTasksProcessor _gameLoadingTasksProcessor;
        private readonly SceneAssetProvider _assetProvider;

        private readonly Dictionary<uint, AudioSourceView> _activeViews;
        private AudioSourceView.Pool _audioPool;
        private GeneralSoundsResource _generalSounds;
        private BGMView _bgmView;
        private bool _subscriptionsActive;
        private bool _poolsReady;
        private bool _resourcesReady;
        private bool _bgmReady;

        public GameAudioPresenter(CombatModel combatModel, ShipModel shipModel,
            IViewPoolsService poolsService, IResourcesService resourcesService,
            GameLoadingTasksProcessor gameLoadingTasksProcessor, SceneAssetProvider assetProvider)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _poolsService = poolsService;
            _resourcesService = resourcesService;
            _gameLoadingTasksProcessor = gameLoadingTasksProcessor;
            _assetProvider = assetProvider;

            _activeViews = new Dictionary<uint, AudioSourceView>();
        }

        public void Initialize()
        {
            if (_gameLoadingTasksProcessor.IsFinished)
            {
                OnLoadingTaskFinished();
            }
            else
            {
                _gameLoadingTasksProcessor.OnTasksFinished += OnLoadingTaskFinished;
            }

            if (_poolsService.IsReady)
            {
                OnPoolsReady();
            }
            else
            {
                _poolsService.Ready += OnPoolsReady;
            }
        }

        public void Dispose()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;
            _poolsService.Ready -= OnPoolsReady;

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


        private void TrySubscribe()
        {
            if (_subscriptionsActive || !_poolsReady || !_resourcesReady)
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

        private void OnLoadingTaskFinished()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;
            _generalSounds = _resourcesService.Get<GeneralSoundsResource>(AddressablesResourcePaths.General.GeneralSounds);
            _resourcesReady = true;
            TryAssignBgm();
            TrySubscribe();
        }

        private void OnPoolsReady()
        {
            _poolsService.Ready -= OnPoolsReady;

            _audioPool = _poolsService.GetPool<AudioSourceView.Pool>();
            _poolsReady = true;
            TrySubscribe();
        }

        private void TryAssignBgm()
        {
            if (_bgmReady)
            {
                return;
            }

            if (!_assetProvider.TryGetLoadedComponent(out _bgmView))
            {
                Debug.LogError("BGMView not provided");
                return;
            }

            _bgmReady = _bgmView != null;
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
