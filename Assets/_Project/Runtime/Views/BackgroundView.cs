using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Settings;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Views
{
    public class BackgroundView : BaseView
    {
        [Inject]
        private BackgroundJitterConfig _conf;

        private Vector3 _baseLocalPos;
        private Vector2 _current, _currentVel;
        private Vector2 _parallax;
        private Vector2 _playerVelocity;
        private float _t, _sx, _sy, _jx, _jy;

        void Awake()
        {
            _baseLocalPos = transform.localPosition;

            _sx = Random.value * 1000f;
            _sy = Random.value * 1000f;
            _jx = Random.value * 2000f;
            _jy = Random.value * 2000f;
        }

        void Update()
        {
            float dt = _conf.UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            _t += dt;

            Vector2 slow =
                new Vector2(Mathf.PerlinNoise(_sx + _t * _conf.SlowFrequency, _sy) - 0.5f,
                    Mathf.PerlinNoise(_sy + _t * _conf.SlowFrequency, _sx) - 0.5f)
                * (_conf.SlowAmplitude * 2f);

            Vector2 jit =
                new Vector2(Mathf.PerlinNoise(_jx + _t * _conf.JitterFrequency, _jy) - 0.5f,
                    Mathf.PerlinNoise(_jy + _t * _conf.JitterFrequency, _jx) - 0.5f)
                * (_conf.JitterAmplitude * 2f);

            Vector2 v = _playerVelocity;
            Vector2 parallaxTarget = -v * _conf.ParallaxStrength;
            parallaxTarget = Vector2.ClampMagnitude(parallaxTarget, _conf.ParallaxMax);

            float a = 1f - Mathf.Exp(-_conf.ParallaxResponse * dt);
            _parallax = Vector2.Lerp(_parallax, parallaxTarget, a);

            Vector2 target = slow + jit + _parallax;
            target = Vector2.ClampMagnitude(target, _conf.MaxOffset);

            _current = Vector2.SmoothDamp(_current, target, ref _currentVel, _conf.SmoothTime, Mathf.Infinity, dt);
            transform.localPosition = _baseLocalPos + (Vector3)_current;
        }

        public void SetPlayerVelocity(Vector2 velocity)
        {
            _playerVelocity = velocity;
        }
    }
}