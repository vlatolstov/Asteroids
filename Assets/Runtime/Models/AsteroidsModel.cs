using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Runtime.Models
{
    public class AsteroidsModel : BaseModel, IInitializable, ITickable, IDisposable
    {
        private readonly IWorldConfig _world;
        private readonly IAsteroidsSpawnConfig _config;

        private float _timer;

        public AsteroidsModel(IWorldConfig world, IAsteroidsSpawnConfig config)
        {
            _world = world;
            _config = config;
        }

        public void Initialize()
        {
            _timer = _config.Interval;

            Subscribe<AsteroidViewOffscreen>(OnOffscreen);
            Subscribe<AsteroidDestroyed>(OnDestroyed);
        }

        public void Dispose()
        {
            Unsubscribe<AsteroidViewOffscreen>(OnOffscreen);
            Unsubscribe<AsteroidDestroyed>(OnDestroyed);
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;

            if (_timer > 0f)
            {
                return;
            }

            SpawnLargeAsteroid();
            _timer = _config.Interval;
        }

        private void OnOffscreen()
        {
            if (!TryGet(out AsteroidViewOffscreen eventData))
            {
                return;
            }

            ChangeData(new AsteroidDespawnRequest(eventData.ViewId));
        }

        private void OnDestroyed()
        {
            if (!TryGet(out AsteroidDestroyed eventData))
            {
                return;
            }

            ChangeData(new AsteroidDespawnRequest(eventData.ViewId));

            if (eventData.Size == AsteroidSize.Large)
            {
                SpawnSmallAsteroids(eventData);
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

            ChangeData(new AsteroidSpawnRequest(_config.Sprite, AsteroidSize.Large, _config.LargeScale, pos, vel, nose, _config.AngleRotationDeg));
        }

        private void SpawnSmallAsteroids(AsteroidDestroyed ev)
        {
            int count = _config.SmallSplit;
            float baseA = Mathf.Atan2(ev.Vel.y, ev.Vel.x);

            for (int i = 0; i < count; i++)
            {
                float spread = (360f / Mathf.Max(2, count)) * Mathf.Deg2Rad;
                float a = baseA + (i - (count - 1) * 0.5f) * spread
                                + Random.Range(-0.25f, 0.25f) * spread;

                float spd = _config.SmallSpeed;
                Vector2 vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * spd;
                float nose = Mathf.Atan2(-vel.x, vel.y);

                ChangeData(new AsteroidSpawnRequest(_config.Sprite, AsteroidSize.Small, _config.SmallScale, ev.Pos, vel, nose, _config.AngleRotationDeg));
            }
        }
    }
}