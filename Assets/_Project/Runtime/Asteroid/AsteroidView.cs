using System;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Movement;
using UnityEngine;

namespace _Project.Runtime.Asteroid
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AsteroidView : BaseMovableView<InertialMotor>
    {
        private AsteroidSize _size;
        private float _selfOffset;
        private bool _entered;

        private SpriteRenderer _sr;

        public event Action<AsteroidDestroyed> Destroyed;
        public event Action<AsteroidOffscreen> Offscreen;

        protected override void Awake()
        {
            base.Awake();
            _sr = GetComponent<SpriteRenderer>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Motor == null)
            {
                return;
            }

            bool inside = Motor.IsInsideWorldRect(_selfOffset);
            switch (_entered)
            {
                case false when inside:
                    Motor.SetWrapMode(true);
                    _entered = true;
                    break;
                case true when !inside:
                    Offscreen?.Invoke(new AsteroidOffscreen(ViewId, _size));
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != gameObject.layer)
            {
                var velocity = Motor?.Velocity ?? Vector2.zero;
                Destroyed?.Invoke(new AsteroidDestroyed(ViewId, _size, transform.position, transform.rotation,
                    transform.localScale, velocity));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer
                && other.CompareTag("Attack"))
            {
                var velocity = Motor?.Velocity ?? Vector2.zero;
                Destroyed?.Invoke(new AsteroidDestroyed(ViewId, _size, transform.position, transform.rotation,
                    transform.localScale, velocity));
            }
        }

        private void Reinitialize(AsteroidSpawnCommand args)
        {
            _size = args.Size;
            _entered = false;
            _sr.sprite = args.Sprite;

            Motor.SetWrapMode(false);
            Motor.SetPose(args.Pos, args.Vel, args.AngleRad);
            ApplyAngularVelocity(args.AngRotation);

            transform.position = args.Pos;
            transform.localScale = new Vector3(args.Scale, args.Scale);
            _selfOffset = Mathf.Max(transform.localScale.x, transform.localScale.y) / 2;
        }

        public void ConfigureMotor(InertialMotor motor)
        {
            SetMotor(motor);
        }

        public readonly struct SpawnArgs
        {
            public readonly AsteroidSpawnCommand Command;
            public readonly InertialMotor Motor;

            public SpawnArgs(AsteroidSpawnCommand command, InertialMotor motor)
            {
                Command = command;
                Motor = motor;
            }
        }

        public class Pool : ViewPool<SpawnArgs, AsteroidView>
        {
            public Pool(Func<AsteroidView> factory, Transform parent, int warmup)
                : base(factory, parent, warmup)
            {
            }

            protected override void Reinitialize(SpawnArgs args, AsteroidView item)
            {
                item.ConfigureMotor(args.Motor);
                item.Reinitialize(args.Command);
            }
        }
    }
}
