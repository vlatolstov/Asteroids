using Runtime.Abstract.Configs;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.Abstract.Movement
{
    public abstract class BaseMotor2D<TConfig> : IMove, IMotorInput where TConfig : class, IMovementConfig
    {
        private readonly TConfig _config;
        private readonly IWorldConfig _world;

        protected BaseMotor2D(TConfig config, IWorldConfig world)
        {
            _config = config;
            _world = world;
        }

        private Vector2 _pos;
        private Vector2 _vel;
        private float _angRad;

        private float _thrust = 0f;
        private float _turnAxis = 0f;

        public Vector2 Position => _pos;
        public Vector2 Velocity => _vel;
        public float AngleRadians => _angRad;
        
        public virtual void SetPose(Vector2 pos, Vector2 vel, float aRad)
        {
            _pos = pos;
            _vel = vel;
            _angRad = aRad;
        }
        
        public void MoveRigidbody(Rigidbody2D rigidbody)
        {
            float dt = Time.fixedDeltaTime;
            UpdateControls(dt);

            Vector2 fwd = new(-Mathf.Sin(_angRad), Mathf.Cos(_angRad));
            _vel += fwd * (_config.Acceleration * Mathf.Clamp01(_thrust) * dt);

            float spd = _vel.magnitude;

            if (spd > _config.MaxSpeed)
            {
                _vel *= (_config.MaxSpeed / spd);
            }

            _angRad -= _config.TurnSpeed * Mathf.Clamp(_turnAxis, -1f, 1f) * dt;

            if (_config.LinearDamping > 0f)
            {
                _vel = Vector2.MoveTowards(_vel, Vector2.zero, _config.LinearDamping * dt);
            }

            Vector2 newPos = _pos + _vel * dt;

            if (_config.IsWrappedByWorldBounds)
            {
                newPos = WrapUtility.Wrap(newPos, _world.WorldRect, _world.WrapOffset);
            }

            rigidbody.MovePosition(newPos);
            rigidbody.MoveRotation(_angRad * Mathf.Rad2Deg);
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