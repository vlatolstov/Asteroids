using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Data;
using _Project.Runtime.Settings;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Models
{
    public class AsteroidsModel : ITickable
    {
        private readonly IWorldConfig _world;
        private readonly AsteroidsSpawnConfig _config;

        public event Action<AsteroidSpawnCommand> AsteroidSpawnRequested;
        public event Action<AsteroidDespawnCommand> AsteroidDespawnRequested;
        public event Action<AsteroidDestroyed> AsteroidDestroyed;

        private int _largeAsteroidsCount;
        private int _smallAsteroidsCount;
        private float _timer;
        private GameState _gameState;

        public AsteroidsModel(IWorldConfig world, AsteroidsSpawnConfig config)
        {
            _world = world;
            _config = config;

            _largeAsteroidsCount = 0;
            _smallAsteroidsCount = 0;
            _timer = _config.Interval;
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;

            if (_timer > 0f || _gameState != GameState.Gameplay)
            {
                return;
            }

            SpawnLargeAsteroid();
            _timer = _config.Interval;
        }
        
        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
        }

        public void HandleAsteroidOffscreen(AsteroidOffscreen offscreen)
        {
            AsteroidDespawnRequested?.Invoke(new AsteroidDespawnCommand(offscreen.ViewId));
        }

        public void HandleAsteroidDestroyed(AsteroidDestroyed destroyed)
        {
            AsteroidDespawnRequested?.Invoke(new AsteroidDespawnCommand(destroyed.ViewId));
            AsteroidDestroyed?.Invoke(destroyed);

            if (destroyed.Size == AsteroidSize.Large)
            {
                _largeAsteroidsCount--;
                SpawnSmallAsteroids(destroyed);
            }
            else
            {
                _smallAsteroidsCount--;
            }
        }

        private void SpawnLargeAsteroid()
        {
            var wr = _world.WorldRect;
            float off = _config.EdgeOffset;
            int side = Random.Range(0, 4);

            var pos = side switch
            {
                0 => new Vector2(wr.xMin - off, Random.Range(wr.yMin, wr.yMax)),
                1 => new Vector2(wr.xMax + off, Random.Range(wr.yMin, wr.yMax)),
                2 => new Vector2(Random.Range(wr.xMin, wr.xMax), wr.yMax + off),
                _ => new Vector2(Random.Range(wr.xMin, wr.xMax), wr.yMin - off),
            };

            var toCenter = wr.center - pos;
            float baseA = Mathf.Atan2(toCenter.y, toCenter.x);
            float jitter = _config.EntryAngleJitterDeg * Mathf.Deg2Rad;
            float a = baseA + Random.Range(-jitter, jitter);

            float spd = _config.LargeSpeed;
            var vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * spd;
            float nose = Mathf.Atan2(-vel.x, vel.y);

            _largeAsteroidsCount++;
            var command = new AsteroidSpawnCommand(_config.Sprite, AsteroidSize.Large, _config.LargeScale, pos, vel,
                nose,
                _config.AngleRotationDeg);

            AsteroidSpawnRequested?.Invoke(command);
        }

        private void SpawnSmallAsteroids(AsteroidDestroyed ev)
        {
            int count = _config.SmallSplit;
            float baseA = Mathf.Atan2(ev.Velocity.y, ev.Velocity.x);

            for (int i = 0; i < count; i++)
            {
                float spread = (360f / Mathf.Max(2, count)) * Mathf.Deg2Rad;
                float a = baseA + (i - (count - 1) * 0.5f) * spread
                                + Random.Range(-0.25f, 0.25f) * spread;

                float spd = _config.SmallSpeed;
                Vector2 vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * spd;
                float nose = Mathf.Atan2(-vel.x, vel.y);

                _smallAsteroidsCount++;
                var command = new AsteroidSpawnCommand(_config.Sprite, AsteroidSize.Small, _config.SmallScale,
                    ev.Position,
                    vel, nose, _config.AngleRotationDeg);
                AsteroidSpawnRequested?.Invoke(command);
            }
        }
    }
}