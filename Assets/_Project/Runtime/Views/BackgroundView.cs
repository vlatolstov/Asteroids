using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Settings;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Views
{
    public class BackgroundView : BaseView
    {
        [Inject]
        private BackgroundJitterConfig _config;

        private Vector3 _baseLocalPosition;
        private Vector2 _currentOffset;
        private Vector2 _currentVelocity;
        private Vector2 _parallaxOffset;
        private Vector2 _playerVelocity;
        private float _time;
        private float _slowNoiseX;
        private float _slowNoiseY;
        private float _jitterNoiseX;
        private float _jitterNoiseY;

        private void Awake()
        {
            _baseLocalPosition = transform.localPosition;

            _slowNoiseX = Random.value * 1000f;
            _slowNoiseY = Random.value * 1000f;
            _jitterNoiseX = Random.value * 2000f;
            _jitterNoiseY = Random.value * 2000f;
        }

        private void Update()
        {
            float dt = _config.UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            if (dt <= 0f)
            {
                return;
            }

            _time += dt;

            var slowOffset =
                new Vector2(Mathf.PerlinNoise(_slowNoiseX + _time * _config.SlowFrequency, _slowNoiseY) - 0.5f,
                    Mathf.PerlinNoise(_slowNoiseY + _time * _config.SlowFrequency, _slowNoiseX) - 0.5f);
            slowOffset *= _config.SlowAmplitude * 2f;

            var jitterOffset =
                new Vector2(Mathf.PerlinNoise(_jitterNoiseX + _time * _config.JitterFrequency, _jitterNoiseY) - 0.5f,
                    Mathf.PerlinNoise(_jitterNoiseY + _time * _config.JitterFrequency, _jitterNoiseX) - 0.5f);
            jitterOffset *= _config.JitterAmplitude * 2f;

            var parallaxTarget = -_playerVelocity * _config.ParallaxStrength;
            parallaxTarget = Vector2.ClampMagnitude(parallaxTarget, _config.ParallaxMax);

            float parallaxLerpFactor = 1f - Mathf.Exp(-_config.ParallaxResponse * dt);
            _parallaxOffset = Vector2.Lerp(_parallaxOffset, parallaxTarget, parallaxLerpFactor);

            var targetOffset = slowOffset + jitterOffset + _parallaxOffset;
            targetOffset = Vector2.ClampMagnitude(targetOffset, _config.MaxOffset);

            _currentOffset = Vector2.SmoothDamp(
                _currentOffset,
                targetOffset,
                ref _currentVelocity,
                _config.SmoothTime,
                Mathf.Infinity,
                dt);

            transform.localPosition = _baseLocalPosition + (Vector3)_currentOffset;
        }

        public void SetPlayerVelocity(Vector2 velocity)
        {
            _playerVelocity = velocity;
        }
    }
}