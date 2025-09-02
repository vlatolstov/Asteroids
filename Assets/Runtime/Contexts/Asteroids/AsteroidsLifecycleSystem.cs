using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Runtime.Contexts.Asteroids
{
    public class AsteroidsLifecycleSystem : IInitializable, IDisposable
    {
        private readonly IModel _model;
        private readonly IAsteroidsSpawnConfig _cfg;

        public AsteroidsLifecycleSystem(IModel model, IAsteroidsSpawnConfig cfg)
        {
            _model = model;
            _cfg = cfg;
        }

        public void Initialize()
        {
            _model.Subscribe<AsteroidViewOffscreen>(OnOffscreen);
            _model.Subscribe<AsteroidViewDestroyed>(OnDestroyed);
        }

        public void Dispose()
        {
            _model.Unsubscribe<AsteroidViewOffscreen>(OnOffscreen);
            _model.Unsubscribe<AsteroidViewDestroyed>(OnDestroyed);
        }

        private void OnOffscreen()
        {
            if (!_model.TryGet(out AsteroidViewOffscreen ev))
            {
                return;
            }

            _model.ChangeData(new AsteroidDespawnRequest(ev.ViewId));
        }

        private void OnDestroyed()
        {
            if (!_model.TryGet(out AsteroidViewDestroyed ev))
            {
                return;
            }

            _model.ChangeData(new AsteroidDespawnRequest(ev.ViewId));

            if (ev.Size == AsteroidSize.Large)
            {
                int count = Random.Range(_cfg.SmallSplitMin, _cfg.SmallSplitMax + 1);
                float baseA = Mathf.Atan2(ev.Vel.y, ev.Vel.x);

                for (int i = 0; i < count; i++)
                {
                    float spread = (360f / Mathf.Max(2, count)) * Mathf.Deg2Rad;
                    float a = baseA + (i - (count - 1) * 0.5f) * spread
                                    + Random.Range(-0.25f, 0.25f) * spread;

                    float spd = Random.Range(_cfg.SmallSpeedMin, _cfg.SmallSpeedMax);
                    Vector2 vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * spd;
                    float nose = Mathf.Atan2(-vel.x, vel.y);
                    
                    _model.ChangeData(new AsteroidSpawnRequest(AsteroidSize.Small, ev.Pos, vel, nose));
                }
            }
        }
    }
}