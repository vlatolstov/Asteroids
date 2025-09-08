using Runtime.Abstract.MVP;
using UnityEngine;

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
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            _t += dt;

            Vector2 slow =
                new Vector2(Mathf.PerlinNoise(_sx + _t * _slowFrequency, _sy) - 0.5f,
                    Mathf.PerlinNoise(_sy + _t * _slowFrequency, _sx) - 0.5f)
                * (_slowAmplitude * 2f);

            Vector2 jit =
                new Vector2(Mathf.PerlinNoise(_jx + _t * _jitterFrequency, _jy) - 0.5f,
                    Mathf.PerlinNoise(_jy + _t * _jitterFrequency, _jx) - 0.5f)
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