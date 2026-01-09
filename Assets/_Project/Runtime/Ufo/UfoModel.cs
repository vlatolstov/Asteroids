using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Services;
using _Project.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Ufo
{
    public class UfoModel : ITickable, IInitializable
    {
        private readonly IResourcesService _resourcesService;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private UfoSpawnResource _assets;
        private UfoSpawnData _data;
        private readonly IWorldConfig _world;

        public event Action<UfoSpawnCommand> UfoSpawnRequested;
        public event Action<uint> UfoDespawnRequested;
        public event Action<UfoDestroyed> UfoDestroyed;

        private float _time;
        private float _nextAt;
        private int _inGame;
        private GameState _gameState;
        private bool _ready;

        public UfoModel(IResourcesService resourcesService, IWorldConfig world,
            IRemoteConfigProvider remoteConfigProvider)
        {
            _resourcesService = resourcesService;
            _world = world;
            _remoteConfigProvider = remoteConfigProvider;

            _time = 0f;
            _inGame = 0;
        }
        
        public void Initialize()
        {
            UniTask.Void(async () =>
            {
                await _resourcesService.LoadAllAsync();
                _assets = _resourcesService.Get<UfoSpawnResource>(AddressablesResourcePaths.General.UfoSpawn);
                if (!_remoteConfigProvider.TryGet(Config.Ufo.Spawn, out _data))
                {
                    Debug.LogWarning("[RemoteConfig] Missing Ufo spawn data.");
                    _data = new UfoSpawnData();
                }

                _nextAt = _data.InitialDelay;
                _ready = true;
            });
        }

        public void Tick()
        {
            if (!_ready)
            {
                return;
            }

            _time += Time.deltaTime;

            if (_time < _nextAt || _gameState != GameState.Gameplay)
            {
                return;
            }

            if (_inGame < _data.MaxAlive)
            {
                SpawnOne();
            }

            _nextAt = _time + _data.Interval;
        }

        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
        }

        public void HandleUfoSpawned(UfoSpawned spawned)
        {
            _inGame++;
        }

        public void HandleUfoOffscreen(uint viewId)
        {
            _inGame = Mathf.Max(0, _inGame - 1);
            UfoDespawnRequested?.Invoke(viewId);
        }

        public void HandleUfoDestroyed(UfoDestroyed destroyed)
        {
            _inGame = Mathf.Max(0, _inGame - 1);
            UfoDestroyed?.Invoke(destroyed);
            UfoDespawnRequested?.Invoke(destroyed.ViewId);
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
                    x = rect.xMin - _data.EdgeOffset;
                    y = Mathf.Lerp(rect.yMin, rect.yMax, t);
                    break;
                case 1:
                    x = rect.xMax + _data.EdgeOffset;
                    y = Mathf.Lerp(rect.yMin, rect.yMax, t);
                    break;
                case 2:
                    x = Mathf.Lerp(rect.xMin, rect.xMax, t);
                    y = rect.yMin - _data.EdgeOffset;
                    break;
                default:
                    x = Mathf.Lerp(rect.xMin, rect.xMax, t);
                    y = rect.yMax + _data.EdgeOffset;
                    break;
            }

            var pos = new Vector2(x, y);

            var toCenter = rect.center - pos;
            var dir = toCenter.sqrMagnitude > 1e-6f ? toCenter.normalized : Vector2.up;

            float jitterDeg = Random.Range(-_data.EntryAngleJitterDeg, _data.EntryAngleJitterDeg);
            dir = GeometryMethods.RotateVector(dir, Mathf.Deg2Rad * jitterDeg);

            Vector2 vel = dir * _data.Speed;
            float angRad = GeometryMethods.DirToAngle(dir);

            var command = new UfoSpawnCommand(_assets.Sprite, _data.Scale, pos, vel, angRad);
            UfoSpawnRequested?.Invoke(command);
        }
    }
}
