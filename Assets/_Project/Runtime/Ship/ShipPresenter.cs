using System;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using UnityEngine;

namespace _Project.Runtime.Ship
{
    public class ShipPresenter : IDisposable
    {
        private readonly ShipModel _shipModel;
        private readonly CombatModel _combatModel;
        private readonly InputModel _inputModel;
        private readonly ShipView.Pool _pool;

        private ShipView _activeShip;

        public ShipPresenter(ShipModel shipModel, CombatModel combatModel, InputModel inputModel, ShipView.Pool pool)
        {
            _shipModel = shipModel;
            _combatModel = combatModel;
            _inputModel = inputModel;
            _pool = pool;

            _inputModel.FireGunPressed += OnGunAttackSignal;
            _inputModel.AoeAttackPressed += OnAoeWeaponAttackSignal;
            _inputModel.ThrustChanged += OnThrustChanged;
            _inputModel.TurnChanged += OnTurnAxisChanged;

            _shipModel.ShipSpawnCommandRequested += OnShipSpawnCommand;
            _shipModel.ShipDespawnCommandRequested += OnShipDespawnCommand;
        }

        public void Dispose()
        {
            _inputModel.FireGunPressed -= OnGunAttackSignal;
            _inputModel.AoeAttackPressed -= OnAoeWeaponAttackSignal;
            _inputModel.ThrustChanged -= OnThrustChanged;
            _inputModel.TurnChanged -= OnTurnAxisChanged;

            _shipModel.ShipSpawnCommandRequested -= OnShipSpawnCommand;
            _shipModel.ShipDespawnCommandRequested -= OnShipDespawnCommand;
            DetachShip();
        }

        private void AttachShip(ShipView shipView)
        {
            if (!shipView)
            {
                return;
            }

            _activeShip = shipView;
            shipView.PoseChanged += OnShipPoseChanged;
            shipView.ProjectileFired += OnProjectileFired;
            shipView.ProjectileWeaponStateChanged += OnProjectileWeaponStateChanged;
            shipView.AoeAttacked += OnAoeAttackReleased;
            shipView.AoeWeaponStateChanged += OnAoeWeaponStateChanged;
            shipView.Destroyed += OnShipDestroyed;
        }

        private void DetachShip()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.PoseChanged -= OnShipPoseChanged;
            _activeShip.ProjectileFired -= OnProjectileFired;
            _activeShip.ProjectileWeaponStateChanged -= OnProjectileWeaponStateChanged;
            _activeShip.AoeAttacked -= OnAoeAttackReleased;
            _activeShip.AoeWeaponStateChanged -= OnAoeWeaponStateChanged;
            _activeShip.Destroyed -= OnShipDestroyed;
            _activeShip = null;
        }

        private void OnThrustChanged(float value)
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.SetupMainEngine(value != 0);
            _activeShip.Motor.SetThrust(value);
        }

        private void OnTurnAxisChanged(float value)
        {
            if (!_activeShip)
            {
                return;
            }

            switch (value)
            {
                case > 0:
                    _activeShip.SetupSideEngines(false, true);
                    break;
                case < 0:
                    _activeShip.SetupSideEngines(true, false);
                    break;
                default:
                    _activeShip.SetupSideEngines(false, false);
                    break;
            }

            _activeShip.Motor.SetTurnAxis(value);
        }

        private void OnGunAttackSignal()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.TryShootProjectile();
        }

        private void OnAoeWeaponAttackSignal()
        {
            if (!_activeShip)
            {
                return;
            }

            _activeShip.TryReleaseAoeAttack();
        }

        private void OnShipSpawnCommand(Vector3 position)
        {
            if (_activeShip)
            {
                return;
            }

            var ship = _pool.Spawn(position);
            AttachShip(ship);
            _shipModel.HandleShipSpawned(new ShipSpawned(ship.transform.position, ship.transform.localScale));
            _shipModel.UpdatePose(new ShipPose(position, Vector2.zero, 0f));
        }

        private void OnShipDespawnCommand()
        {
            if (!_activeShip)
            {
                return;
            }

            _pool.Despawn(_activeShip);
            DetachShip();
        }

        private void OnShipPoseChanged(ShipPose shipPose)
        {
            _shipModel.UpdatePose(shipPose);
        }

        private void OnProjectileWeaponStateChanged(ProjectileWeaponState state)
        {
            _shipModel.UpdateProjectileWeaponState(state);
        }

        private void OnAoeWeaponStateChanged(AoeWeaponState state)
        {
            _shipModel.UpdateAoeWeaponState(state);
        }

        private void OnShipDestroyed(ShipDestroyed destroyed)
        {
            _shipModel.HandleShipDestroyed(destroyed);
        }

        private void OnProjectileFired(ProjectileShot shot)
        {
            _combatModel.HandleProjectileShot(shot);
        }

        private void OnAoeAttackReleased(AoeAttackReleased attack)
        {
            _combatModel.HandleAoeAttackReleased(attack);
        }
    }
}