using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Settings;
using _Project.Runtime.Views;

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
            var view =_audioPool.Spawn(shot.Position, shot.Weapon.AttackSound);
            RegisterView(view);
        }

        private void OnProjectileHit(ProjectileHit hit)
        {
            var view =_audioPool.Spawn(hit.Position, hit.Projectile.HitSound);
            RegisterView(view);
        }

        private void OnAoeAttackReleased(AoeAttackReleased attack)
        {
            var view =_audioPool.Spawn(attack.Emitter.position, attack.Weapon.AttackSound);
            RegisterView(view);
        }

        private void OnAoeHit(AoeHit hit)
        {
            var view = _audioPool.Spawn(hit.Position, hit.Attack.HitSound);
            RegisterView(view);
        }

        private void OnShipSpawned(ShipSpawned spawned)
        {
            var view =_audioPool.Spawn(spawned.Position, _generalSounds.ShipSpawn);
            RegisterView(view);
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