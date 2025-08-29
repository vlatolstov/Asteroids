using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Movement
{
    //TODO change spawning system
    public sealed class PlayerShipSpawner : IInitializable, IDisposable
    {
        private readonly ShipView.Pool _pool;
        private readonly IModel _model;
        private readonly IWorldConfig _world;
        private readonly Vector2 _spawnPos;

        private ShipView _instance;

        public PlayerShipSpawner(
            ShipView.Pool pool,
            IModel model,
            IWorldConfig world)
        {
            _pool = pool;
            _model = model;
            _world = world;
        }

        public void Initialize()
        {
            // Стартовую позу кладём в модель (если нужно — центр мира)
            var pos = new Vector2(0,0);
            _model.ChangeData<ShipPose>(_ => new ShipPose { Position = pos, Velocity = Vector2.zero, AngleRadians = 0f });

            // Спавним вью (в Pool.OnSpawned она автоматически попадёт в ViewsContainer)
            _instance = _pool.Spawn();

            // Принудительно выставим позу в мотор (если уже есть)
            var motor = _instance.GetComponent<ShipMotor>(); // RigidBodyMotor2D<ShipConfig>
            if (motor != null)
                motor.SetPose(pos, Vector2.zero, 0f);
        }

        public void Dispose()
        {
            if (_instance != null)
            {
                _pool.Despawn(_instance);
                _instance = null;
            }
        }
    }
}