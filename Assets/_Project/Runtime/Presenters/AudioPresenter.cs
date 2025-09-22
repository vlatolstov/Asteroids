using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.Views;
using UnityEngine;

namespace _Project.Runtime.Presenters
{
    public class AudioPresenter : IDisposable
    {
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;

        private readonly AudioSourceView.Pool _audioPool;
        private readonly GeneralSoundsConfig _generalSounds;

        private readonly Dictionary<uint, AudioSourceView> _activeViews;

        public AudioPresenter(CombatModel combatModel, ShipModel shipModel,
            GeneralSoundsConfig generalSounds, AudioSourceView.Pool audioPool)
        {
            _combatModel = combatModel;
            _shipModel = shipModel;
            _generalSounds = generalSounds;
            _audioPool = audioPool;

            _activeViews = new Dictionary<uint, AudioSourceView>();

            _combatModel.ProjectileShot += OnProjectileShot;
            _combatModel.ProjectileHit += OnProjectileHit;
            _combatModel.AoeAttackReleased += OnAoeAttackReleased;
            _combatModel.AoeHit += OnAoeHit;

            _shipModel.ShipSpawned += OnShipSpawned;
        }

        public void Dispose()
        {
            _combatModel.ProjectileShot -= OnProjectileShot;
            _combatModel.ProjectileHit -= OnProjectileHit;
            _combatModel.AoeAttackReleased -= OnAoeAttackReleased;
            _combatModel.AoeHit -= OnAoeHit;

            _shipModel.ShipSpawned -= OnShipSpawned;
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
            _audioPool.Despawn(view);
        }
        
        private void SpawnAndRegister(Vector2 position, AudioClip clip)
        {
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