using Runtime.Abstract.Configs;
using Runtime.Utils;
using UnityEngine;
using Zenject;

namespace Runtime.Abstract.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseMotor2D<TConfig> : MonoBehaviour, IMove, IMotorInput
        where TConfig : class, IMovementConfig
    {
        [Inject]
        protected TConfig Config;

        [Inject]
        protected IWorldConfig World;

        private Rigidbody2D _rb;
        private Vector2 _vel;
        private float _angRad;

        private float _thrust = 0f;
        private float _turnAxis = 0f;

        public Vector2 Position => _rb ? _rb.position : transform.position;
        public Vector2 Velocity => _vel;
        public float AngleRadians => _angRad;

        public virtual void SetPose(Vector2 pos, Vector2 vel, float aRad)
        {
            _vel = vel;
            _angRad = aRad;
            float angDegrees = aRad * Mathf.Rad2Deg;
            if (_rb)
            {
                _rb.position = pos;
                _rb.MoveRotation(angDegrees);
            }
            else
            {
                transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angDegrees));
            }
        }

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }

        protected virtual void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            UpdateControls(dt);

            Vector2 fwd = new(-Mathf.Sin(_angRad), Mathf.Cos(_angRad));
            _vel += fwd * (Config.Acceleration * Mathf.Clamp01(_thrust) * dt);

            float spd = _vel.magnitude;

            if (spd > Config.MaxSpeed)
            {
                _vel *= (Config.MaxSpeed / spd);
            }

            _angRad -= Config.TurnSpeed * Mathf.Clamp(_turnAxis, -1f, 1f) * dt;

            if (Config.LinearDamping > 0f)
            {
                _vel = Vector2.MoveTowards(_vel, Vector2.zero, Config.LinearDamping * dt);
            }

            Vector2 newPos = Position + _vel * dt;

            if (Config.IsWrappedByWorldBounds)
            {
                newPos = WrapUtility.Wrap(newPos, World.WorldRect, World.WrapOffset);
            }

            _rb.MovePosition(newPos);
            _rb.MoveRotation(_angRad * Mathf.Rad2Deg);
        }

        public void SetControls(float thrust, float turnAxis)
        {
            _thrust = thrust;
            _turnAxis = turnAxis;
        }

        protected virtual void UpdateControls(float dt)
        { }
    }
}