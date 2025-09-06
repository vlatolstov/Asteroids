using System.Collections.Generic;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Utils;
using UnityEngine;
using Zenject;

namespace Runtime.Models
{
    public class UfoModel : BaseModel, IInitializable, ITickable
    {
        private readonly IUfoSpawnConfig _spawnConfig;
        private readonly IWorldConfig _world;

        private float _time;
        private float _nextAt;
        private int _alive;

        public UfoModel(IUfoSpawnConfig spawnConfig, IWorldConfig world)
        {
            _spawnConfig = spawnConfig;
            _world = world;
        }

        public void Initialize()
        {
            _time = 0f;
            _alive = 0;
            _nextAt = _spawnConfig.InitialDelay;

            Subscribe<UfoSpawned>(OnUfoSpawned);
            Subscribe<UfoDestroyed>(OnUfoDestroyed);
        }

        public void Tick()
        {
            _time += Time.deltaTime;

            if (_time < _nextAt)
            {
                return;
            }

            if (_alive < _spawnConfig.MaxAlive)
            {
                SpawnOne();
            }

            _nextAt = _time + _spawnConfig.Interval;
        }

        public void OnUfoSpawned()
        {
            if (!TryGet(out UfoSpawned spawned))
            {
                return;
            }

            _alive++;
        }

        public void OnUfoDestroyed()
        {
            if (!TryGet(out UfoDestroyed destroyed))
            {
                return;
            }

            _alive--;
            Publish(new UfoDespawnCommand(destroyed.ViewId));
        }

        private void SpawnOne()
        {
            var rect = _world.WorldRect;

            int side = Random.Range(0, 4);

            float x, y;
            float t = Random.Range(0, 1f);

            switch (side)
            {
                case 0:
                    x = rect.xMin - _spawnConfig.EdgeOffset;
                    y = Mathf.Lerp(rect.yMin, rect.yMax, t);
                    break;
                case 1:
                    x = rect.xMax + _spawnConfig.EdgeOffset;
                    y = Mathf.Lerp(rect.yMin, rect.yMax, t);
                    break;
                case 2:
                    x = Mathf.Lerp(rect.xMin, rect.xMax, t);
                    y = rect.yMin - _spawnConfig.EdgeOffset;
                    break;
                default:
                    x = Mathf.Lerp(rect.xMin, rect.xMax, t);
                    y = rect.yMax + _spawnConfig.EdgeOffset;
                    break;
            }

            var pos = new Vector2(x, y);

            var toCenter = rect.center - pos;
            var dir = toCenter.sqrMagnitude > 1e-6f ? toCenter.normalized : Vector2.up;

            float jitterDeg = Random.Range(-_spawnConfig.EntryAngleJitterDeg, _spawnConfig.EntryAngleJitterDeg);
            dir = GeometryMethods.RotateVector(dir, Mathf.Deg2Rad * jitterDeg);

            Vector2 vel = dir * _spawnConfig.Speed;
            float angRad = GeometryMethods.DirToAngle(dir);

            Publish(new UfoSpawnCommand(_spawnConfig.Sprite, _spawnConfig.Scale, pos, vel, angRad));
        }
    }
}