using Runtime.Abstract.MVP;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Views
{
    public class BackgroundView : BaseView
    {
        [Header("Random drift")]
        [SerializeField]
        private float _slowAmplitude = 0.2f;

        [SerializeField]
        private float _slowFrequency = 0.1f;

        [SerializeField]
        private float _jitterAmplitude = 0.05f;

        [SerializeField]
        private float _jitterFrequency = 2.0f;

        [Header("Parallax vs player velocity")]
        [SerializeField]
        private float _parallaxStrength = 0.02f;

        [SerializeField]
        private float _parallaxResponse = 6f;

        [SerializeField]
        private float _parallaxMax = 0.5f;

        [Header("Smoothing / Limits")]
        [SerializeField]
        private float _smoothTime = 0.2f;

        [SerializeField]
        private float _maxOffset = 40f;

        [SerializeField]
        private bool _useUnscaledTime = false;

        Vector3 _baseLocalPos;
        Vector2 _current, _currentVel;
        Vector2 _parallax;
        Vector2 _playerVelocity;
        float _t, sx, sy, jx, jy;

        void Awake()
        {
            _baseLocalPos = transform.localPosition;

            sx = Random.value * 1000f;
            sy = Random.value * 1000f;
            jx = Random.value * 2000f;
            jy = Random.value * 2000f;
        }

        void Update()
        {
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            _t += dt;

            Vector2 slow =
                new Vector2(Mathf.PerlinNoise(sx + _t * _slowFrequency, sy) - 0.5f,
                    Mathf.PerlinNoise(sy + _t * _slowFrequency, sx) - 0.5f)
                * (_slowAmplitude * 2f);

            Vector2 jit =
                new Vector2(Mathf.PerlinNoise(jx + _t * _jitterFrequency, jy) - 0.5f,
                    Mathf.PerlinNoise(jy + _t * _jitterFrequency, jx) - 0.5f)
                * (_jitterAmplitude * 2f);

            Vector2 v = _playerVelocity;
            Vector2 parallaxTarget = -v * _parallaxStrength;
            parallaxTarget = Vector2.ClampMagnitude(parallaxTarget, _parallaxMax);

            float a = 1f - Mathf.Exp(-_parallaxResponse * dt);
            _parallax = Vector2.Lerp(_parallax, parallaxTarget, a);

            Vector2 target = slow + jit + _parallax;
            target = Vector2.ClampMagnitude(target, _maxOffset);

            _current = Vector2.SmoothDamp(_current, target, ref _currentVel, _smoothTime, Mathf.Infinity, dt);
            transform.localPosition = _baseLocalPos + (Vector3)_current;
        }

        public void SetPlayerVelocity(Vector2 velocity)
        {
            _playerVelocity = velocity;
        }
    }
}