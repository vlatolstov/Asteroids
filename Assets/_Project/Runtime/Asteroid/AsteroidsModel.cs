using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Asteroid
{
    public class AsteroidsModel : ITickable, IInitializable
    {
        private readonly IWorldConfig _world;
        private readonly IConfigsService _configsService;
        private readonly IRemoteConfigProvider _remoteConfigProvider;
        private AsteroidsSpawnConfig _assets;
        private AsteroidsSpawnData _data;

        public event Action<AsteroidSpawnCommand> AsteroidSpawnRequested;
        public event Action<AsteroidDespawnCommand> AsteroidDespawnRequested;
        public event Action<AsteroidDestroyed> AsteroidDestroyed;
        
        private int _largeInGameCount;
        private int _smallInGameCount;

        private float _timer;
        private GameState _gameState;
        private bool _ready;

        public AsteroidsModel(IWorldConfig world, IConfigsService configsService,
            IRemoteConfigProvider remoteConfigProvider)
        {
            _world = world;
            _configsService = configsService;
            _remoteConfigProvider = remoteConfigProvider;
        }
        
        public void Initialize()
        {
            UniTask.Void(async () =>
            {
                await _configsService.LoadAllAsync();
                _assets = _configsService.Get<AsteroidsSpawnConfig>(AddressablesConfigPaths.General.AsteroidsSpawn);
                if (!_remoteConfigProvider.TryGet(Config.Asteroids.Spawn, out _data))
                {
                    Debug.LogWarning("[RemoteConfig] Missing asteroids spawn data.");
                    _data = new AsteroidsSpawnData();
                }

                _timer = _data.Interval;
                _ready = true;
            });
        }

        public void Tick()
        {
            if (!_ready)
            {
                return;
            }

            _timer -= Time.deltaTime;

            if (_timer > 0 || _gameState != GameState.Gameplay)
            {
                return;
            }

            SpawnLargeAsteroid();
            _timer = _data.Interval;
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
                SpawnSmallAsteroids(destroyed);
                _largeInGameCount--;
            }
            else
            {
                _smallInGameCount--;
            }
        }

        private void SpawnLargeAsteroid()
        {
            var worldRect = _world.WorldRect;
            float edgeOffset = _data.EdgeOffset;
            int entrySideIndex = Random.Range(0, 4);

            Vector2 spawnPosition = entrySideIndex switch
            {
                0   => new Vector2(worldRect.xMin - edgeOffset, Random.Range(worldRect.yMin, worldRect.yMax)),
                1  => new Vector2(worldRect.xMax + edgeOffset, Random.Range(worldRect.yMin, worldRect.yMax)),
                2    => new Vector2(Random.Range(worldRect.xMin, worldRect.xMax), worldRect.yMax + edgeOffset),
                3 => new Vector2(Random.Range(worldRect.xMin, worldRect.xMax), worldRect.yMin - edgeOffset),
                _               => new Vector2(worldRect.center.x, worldRect.center.y) 
            };

            Vector2 toWorldCenter = worldRect.center - spawnPosition;
            float baseAngleToCenterRad = Mathf.Atan2(toWorldCenter.y, toWorldCenter.x);

            float entryAngleJitterRad = _data.EntryAngleJitterDeg * Mathf.Deg2Rad;
            float entryAngleRad = baseAngleToCenterRad + Random.Range(-entryAngleJitterRad, entryAngleJitterRad);

            float largeAsteroidSpeed = Random.Range(Mathf.Max(0f, _data.EntrySpeedMin),
                Mathf.Max(0f, _data.EntrySpeedMax));
            Vector2 velocity = new Vector2(Mathf.Cos(entryAngleRad), Mathf.Sin(entryAngleRad)) * largeAsteroidSpeed;

            float noseAngleRad = Mathf.Atan2(-velocity.x, velocity.y);

            var spawnCommand = new AsteroidSpawnCommand(
                _assets.Sprite,
                AsteroidSize.Large,
                _data.LargeScale,
                spawnPosition,
                velocity,
                noseAngleRad,
                Random.Range(_data.RotationMinDeg, _data.RotationMaxDeg));

            _largeInGameCount++;
            AsteroidSpawnRequested?.Invoke(spawnCommand);
        }

        private void SpawnSmallAsteroids(AsteroidDestroyed destroyedEvent)
        {
            const float fullCircleDegrees = 360f;
            const float minFragmentsForSpread = 2f;
            const float randomSpreadHalfWidth = 0.25f;
            const float degreesToRadians = Mathf.Deg2Rad;

            int fragmentCount = Random.Range(Mathf.Max(0, _data.SmallSplitMin),
                Mathf.Max(0, _data.SmallSplitMax) + 1);
            float baseDirectionRad = Mathf.Atan2(destroyedEvent.Velocity.y, destroyedEvent.Velocity.x);

            float spreadStepRad = (fullCircleDegrees / Mathf.Max(minFragmentsForSpread, fragmentCount)) * degreesToRadians;

            for (int i = 0; i < fragmentCount; i++)
            {
                float indexCentered = i - (fragmentCount - 1) * 0.5f;

                float randomJitterRad = Random.Range(-randomSpreadHalfWidth, randomSpreadHalfWidth) * spreadStepRad;

                float fragmentAngleRad = baseDirectionRad + indexCentered * spreadStepRad + randomJitterRad;

                float smallAsteroidSpeed = Random.Range(Mathf.Max(0f, _data.SmallSpeedMin),
                    Mathf.Max(0f, _data.SmallSpeedMax));
                Vector2 fragmentVelocity = new Vector2(Mathf.Cos(fragmentAngleRad), Mathf.Sin(fragmentAngleRad)) * smallAsteroidSpeed;

                float noseAngleRad = Mathf.Atan2(-fragmentVelocity.x, fragmentVelocity.y);

                var spawnCommand = new AsteroidSpawnCommand(
                    _assets.Sprite,
                    AsteroidSize.Small,
                    _data.SmallScale,
                    destroyedEvent.Position,
                    fragmentVelocity,
                    noseAngleRad,
                    Random.Range(_data.RotationMinDeg, _data.RotationMaxDeg));
                
                _smallInGameCount++;
                AsteroidSpawnRequested?.Invoke(spawnCommand);
            }
        }
    }
}
