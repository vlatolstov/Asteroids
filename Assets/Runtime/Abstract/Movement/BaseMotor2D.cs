using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Utils;
using UnityEngine;
using Zenject;

namespace Runtime.Abstract.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseMotor2D<TConfig> : MonoBehaviour, IMove
        where TConfig : class, IMovementConfig
    {
        [Inject]
        protected TConfig Config;

        [Inject]
        protected IWorldConfig World;

        [Inject]
        protected IModel Model;

        protected Rigidbody2D Rb;
        protected Vector2 Vel;
        protected float AngRad;

        public Vector2 Position => Rb ? Rb.position : transform.position;
        public Vector2 Velocity => Vel;
        public float AngleRadians => AngRad;

        public virtual void SetPose(Vector2 pos, Vector2 vel, float aRad)
        {
            Vel = vel;
            AngRad = aRad;
            float angDegrees = aRad * Mathf.Rad2Deg;
            if (Rb)
            {
                Rb.position = pos;
                Rb.MoveRotation(angDegrees);
            }
            else
            {
                transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angDegrees));
            }
        }

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Rb.bodyType = RigidbodyType2D.Kinematic;
            Rb.gravityScale = 0f;
            Rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }

        protected virtual void Start()
        {
            if (!TryReadPoseFromModel(out var pos, out var vel, out var aRad))
            {
                pos = transform.position;
                vel = Vector2.zero;
                aRad = transform.eulerAngles.z * Mathf.Deg2Rad;
                WritePoseToModel(pos, vel, aRad);
            }

            SetPose(pos, vel, aRad);
        }

        protected virtual void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            var (thrust, turnAxis) = ReadControlAxes();

            Vector2 fwd = new(-Mathf.Sin(AngRad), Mathf.Cos(AngRad));
            Vel += fwd * (Config.Acceleration * Mathf.Clamp01(thrust) * dt);

            float spd = Vel.magnitude;
            
            if (spd > Config.MaxSpeed)
            {
                Vel *= (Config.MaxSpeed / spd);
            }

            AngRad -= Config.TurnSpeed * Mathf.Clamp(turnAxis, -1f, 1f) * dt;

            if (Config.LinearDamping > 0f)
            {
                Vel = Vector2.MoveTowards(Vel, Vector2.zero, Config.LinearDamping * dt);
            }

            Vector2 newPos = Position + Vel * dt;
            newPos = WrapUtility.Wrap(newPos, World.WorldRect);

            Rb.MovePosition(newPos);
            Rb.MoveRotation(AngRad * Mathf.Rad2Deg);

            WritePoseToModel(newPos, Vel, AngRad);
        }

        protected abstract (float thrust, float turnAxis) ReadControlAxes();
        protected abstract bool TryReadPoseFromModel(out Vector2 pos, out Vector2 vel, out float angleRad);
        protected abstract void WritePoseToModel(Vector2 pos, Vector2 vel, float angleRad);
    }
}