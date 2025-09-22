using System;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Movement;
using _Project.Runtime.Views;
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
                Destroyed?.Invoke(new AsteroidDestroyed(ViewId, _size, transform.position, transform.rotation,
                    transform.localScale, Motor.Velocity));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer
                && other.CompareTag("Attack"))
            {
                Destroyed?.Invoke(new AsteroidDestroyed(ViewId, _size, transform.position, transform.rotation,
                    transform.localScale, Motor.Velocity));
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

        public class Pool : ViewPool<AsteroidSpawnCommand, AsteroidView>
        {
            public Pool(ViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(AsteroidSpawnCommand args, AsteroidView item)
            {
                item.Reinitialize(args);
                base.Reinitialize(args, item);
            }
        }
    }
}