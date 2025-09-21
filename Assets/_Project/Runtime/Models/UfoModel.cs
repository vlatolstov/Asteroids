using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Settings;
using _Project.Runtime.Utils;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Models
{
    public class UfoModel : BaseModel, IInitializable, ITickable
    {
        private readonly UfoSpawnConfig _spawnConfig;
        private readonly IWorldConfig _world;

        private float _time;
        private float _nextAt;
        private int _alive;
        private GameState _gameState = GameState.Gameplay;

        public UfoModel(UfoSpawnConfig spawnConfig, IWorldConfig world)
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
            
            if (_gameState != GameState.Gameplay)
            {
                return;
            }

            if (_alive < _spawnConfig.MaxAlive)
            {
                SpawnOne();
            }

            _nextAt = _time + _spawnConfig.Interval;
        }

        private void OnUfoSpawned()
        {
            if (!TryGet(out UfoSpawned spawned))
            {
                return;
            }

            _alive++;
        }

        private void OnUfoDestroyed()
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
        
        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
        }
    }
}