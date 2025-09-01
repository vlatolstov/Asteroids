using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Asteroids
{
    public class AsteroidSpawnSystem : ITickable, IInitializable
    {
        private readonly IModel _model;
        private readonly IWorldConfig _world;
        private readonly IAsteroidsSpawnConfig _config;
        private int _nextId = 1;
        private float _timer;

        public AsteroidSpawnSystem(IModel model, IWorldConfig world, IAsteroidsSpawnConfig config)
        {
            _model = model;
            _world = world;
            _config = config;
        }

        public void Initialize()
        {
            _timer = _config.Interval;
        }

        public void Tick()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f)
            {
                return;
            }
            
            _timer = _config.Interval;

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

            Vector2 toCenter = wr.center - pos;
            float baseA = Mathf.Atan2(toCenter.y, toCenter.x);
            float jitter = _config.EntryAngleJitterDeg * Mathf.Deg2Rad;
            float a = baseA + Random.Range(-jitter, jitter);

            float spd = Random.Range(_config.EntrySpeedMin, _config.EntrySpeedMax);
            Vector2 vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * spd;
            float nose = Mathf.Atan2(-vel.x, vel.y);

            var id = new AsteroidId(_nextId++);
            _model.ChangeData(new AsteroidSpawnRequest(id, AsteroidSize.Large, pos, vel, nose));
        }
    }
}