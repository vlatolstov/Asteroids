using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Data;
using UnityEngine;

namespace _Project.Runtime.Models
{
    public class ShipModel
    {
        public event Action<Vector3> ShipSpawnCommandRequested;
        public event Action ShipDespawnCommandRequested;
        public event Action<ShipSpawned> ShipSpawned;
        public event Action<ShipDestroyed> ShipDestroyed;
        public event Action<ShipPose> ShipPoseChanged;
        public event Action<ProjectileWeaponState> ProjectileWeaponStateChanged;
        public event Action<AoeWeaponState> AoeWeaponStateChanged;

        public bool ShipInGame { get; private set; }

        public ShipPose CurShipPose { get; private set; }
        
        private ProjectileWeaponState _curProjWeaponState;
        private AoeWeaponState _curAoeWeaponState;


        private readonly IWorldConfig _worldConfig;

        public ShipModel(IWorldConfig worldConfig)
        {
            _worldConfig = worldConfig;
        }

        public void RequestSpawn()
        {
            if (ShipInGame)
            {
                return;
            }

            ShipSpawnCommandRequested?.Invoke(_worldConfig.WorldRect.center);
        }

        public void RequestDespawn()
        {
            ShipInGame = false;
            ShipDespawnCommandRequested?.Invoke();
        }

        public void HandleShipSpawned(ShipSpawned shipSpawned)
        {
            ShipInGame = true;
            ShipSpawned?.Invoke(shipSpawned);
        }

        public void HandleShipDestroyed(ShipDestroyed shipDestroyed)
        {
            ShipInGame = false;
            ShipDestroyed?.Invoke(shipDestroyed);
            ShipDespawnCommandRequested?.Invoke();
        }

        public void UpdatePose(ShipPose pose)
        {
            CurShipPose = pose;
            ShipPoseChanged?.Invoke(pose);
        }

        public void UpdateProjectileWeaponState(ProjectileWeaponState state)
        {
            _curProjWeaponState = state;
            ProjectileWeaponStateChanged?.Invoke(_curProjWeaponState);
        }

        public void UpdateAoeWeaponState(AoeWeaponState state)
        {
            _curAoeWeaponState = state;
            AoeWeaponStateChanged?.Invoke(_curAoeWeaponState);
        }
    }
}