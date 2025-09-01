using System;
using Runtime.Abstract.Configs;
using Runtime.Movement;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Spawn
{
    //TODO change spawning system 
    public sealed class PlayerShipSpawner : IInitializable, IDisposable
    {
        private readonly ShipView.Pool _pool;
        private ShipView _instance;

        public PlayerShipSpawner(
            ShipView.Pool pool)
        {
            _pool = pool;
        }

        public void Initialize()
        {
            var pos = new Vector2(0,0);
            
            _instance = _pool.Spawn();

            var motor = _instance.GetComponent<ShipMotor>();
            if (motor)
                motor.SetPose(pos, Vector2.zero, 0f);
        }

        public void Dispose()
        {
            if (_instance)
            {
                _pool.Despawn(_instance);
                _instance = null;
            }
        }
    }
}