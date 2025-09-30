using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Movement;
using _Project.Runtime.Utils;
using UnityEngine;

namespace _Project.Runtime.Abstract.Movement
{
    public abstract class BaseMotor2D
    {
        private float _thrust;
        private float _turnAxis;
        private bool _wrapEnabled;
        
        protected readonly MovementConfig Config;
        protected readonly IWorldConfig World;
        
        protected BaseMotor2D(MovementConfig config, IWorldConfig world)
        {
            Config = config;
            World = world;
        }

        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float AngleRadians { get; private set; }

        public void SetPose(Vector2 pos, Vector2 vel, float aRad)
        {
            Position = pos;
            Velocity = vel;
            AngleRadians = aRad;
        }

        public void MoveRigidbody(Rigidbody2D rigidbody)
        {
            float dt = Time.fixedDeltaTime;
            UpdateControls(dt);

            Vector2 forward = new(-Mathf.Sin(AngleRadians), Mathf.Cos(AngleRadians));
            Velocity += forward * (Config.Acceleration * Mathf.Clamp01(_thrust) * dt);

            float speed = Velocity.magnitude;

            if (speed > Config.MaxSpeed)
            {
                Velocity *= (Config.MaxSpeed / speed);
            }

            AngleRadians -= Config.TurnSpeed * Mathf.Clamp(_turnAxis, -1f, 1f) * dt;

            if (Config.LinearDamping > 0f)
            {
                Velocity = Vector2.MoveTowards(Velocity, Vector2.zero, Config.LinearDamping * dt);
            }

            Position += Velocity * dt;

            if (Config.IsWrappedByWorldBounds && _wrapEnabled)
            {
                Position = GeometryMethods.Wrap(Position, World.WorldRect, World.WrapOffset);
            }

            rigidbody.MovePosition(Position);
            ApplyRotation(rigidbody, AngleRadians);
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
            _wrapEnabled = wrap;
        }

        public bool IsInsideWorldRect(float? selfOffset = null)
        {
            return World.ExpandedRect(selfOffset).Contains(Position);
        }
    }
}