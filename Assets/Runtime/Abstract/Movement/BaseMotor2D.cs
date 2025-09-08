using Runtime.Abstract.Configs;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.Abstract.Movement
{
    public abstract class BaseMotor2D<TConfig> : IMove, IMotorInput, IWrapByWorldBounds
        where TConfig : class, IMovementConfig
    {
        protected readonly TConfig Config;
        protected readonly IWorldConfig World;

        protected BaseMotor2D(TConfig config, IWorldConfig world)
        {
            Config = config;
            World = world;
        }

        private Vector2 _pos;
        private Vector2 _vel;
        private float _angRad;

        protected bool WrapEnabled;

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

            _pos += _vel * dt;

            if (Config.IsWrappedByWorldBounds && WrapEnabled)
            {
                _pos = GeometryMethods.Wrap(_pos, World.WorldRect, World.WrapOffset);
            }

            rigidbody.MovePosition(_pos);
            ApplyRotation(rigidbody, _angRad);
        }

        protected virtual void ApplyRotation(Rigidbody2D rb, float angleRad)
        {
            rb.MoveRotation(angleRad * Mathf.Rad2Deg);
        }

        protected virtual void UpdateControls(float dt)
        { }

        public void SetThrust(float thrust)
        {
            _thrust = thrust;
        }

        public void SetTurnAxis(float turnAxis)
        {
            _turnAxis = turnAxis;
        }

        public void SetWrapMode(bool wrap)
        {
            WrapEnabled = wrap;
        }

        public bool IsInsideWorldRect(float? selfOffset = null)
        {
            return World.ExpandedRect(selfOffset).Contains(Position);
        }
    }
}