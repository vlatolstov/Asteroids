using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Data;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Asteroid
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

            if (_timer > 0 || _gameState != GameState.Gameplay)
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
            var worldRect = _world.WorldRect;
            float edgeOffset = _config.EdgeOffset;
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

            float entryAngleJitterRad = _config.EntryAngleJitterDeg * Mathf.Deg2Rad;
            float entryAngleRad = baseAngleToCenterRad + Random.Range(-entryAngleJitterRad, entryAngleJitterRad);

            float largeAsteroidSpeed = _config.LargeSpeed;
            Vector2 velocity = new Vector2(Mathf.Cos(entryAngleRad), Mathf.Sin(entryAngleRad)) * largeAsteroidSpeed;

            float noseAngleRad = Mathf.Atan2(-velocity.x, velocity.y);

            _largeAsteroidsCount++;

            var spawnCommand = new AsteroidSpawnCommand(
                _config.Sprite,
                AsteroidSize.Large,
                _config.LargeScale,
                spawnPosition,
                velocity,
                noseAngleRad,
                _config.AngleRotationDeg);

            AsteroidSpawnRequested?.Invoke(spawnCommand);
        }

        private void SpawnSmallAsteroids(AsteroidDestroyed destroyedEvent)
        {
            const float fullCircleDegrees = 360f;
            const float minFragmentsForSpread = 2f;
            const float randomSpreadHalfWidth = 0.25f;
            const float degreesToRadians = Mathf.Deg2Rad;

            int fragmentCount = _config.SmallSplit;
            float baseDirectionRad = Mathf.Atan2(destroyedEvent.Velocity.y, destroyedEvent.Velocity.x);

            float spreadStepRad = (fullCircleDegrees / Mathf.Max(minFragmentsForSpread, fragmentCount)) * degreesToRadians;

            for (int i = 0; i < fragmentCount; i++)
            {
                float indexCentered = i - (fragmentCount - 1) * 0.5f;

                float randomJitterRad = Random.Range(-randomSpreadHalfWidth, randomSpreadHalfWidth) * spreadStepRad;

                float fragmentAngleRad = baseDirectionRad + indexCentered * spreadStepRad + randomJitterRad;

                float smallAsteroidSpeed = _config.SmallSpeed;
                Vector2 fragmentVelocity = new Vector2(Mathf.Cos(fragmentAngleRad), Mathf.Sin(fragmentAngleRad)) * smallAsteroidSpeed;

                float noseAngleRad = Mathf.Atan2(-fragmentVelocity.x, fragmentVelocity.y);

                _smallAsteroidsCount++;

                var spawnCommand = new AsteroidSpawnCommand(
                    _config.Sprite,
                    AsteroidSize.Small,
                    _config.SmallScale,
                    destroyedEvent.Position,
                    fragmentVelocity,
                    noseAngleRad,
                    _config.AngleRotationDeg);

                AsteroidSpawnRequested?.Invoke(spawnCommand);
            }
        }
    }
}
