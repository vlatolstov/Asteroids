using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AsteroidView : BaseMovableView<InertialMotor>
    {
        private SpriteRenderer _sr;
        private AsteroidSize _size;
        private bool _entered;

        [Inject]
        private IWorldConfig _world;


        protected override void Awake()
        {
            base.Awake();
            _sr = GetComponent<SpriteRenderer>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y);
            bool inside = _world.ExpandedRect(maxScale / 2 + 1).Contains(Motor.Position);

            switch (_entered)
            {
                case false when inside:
                    Motor.SetWrapMode(true);
                    _entered = true;
                    break;
                case true when !inside:
                    ReportOffscreen();
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != gameObject.layer)
            {
                ReportDestroyedByHit();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer
                && other.CompareTag("Attack"))
            {
                ReportDestroyedByHit();
            }
        }

        private void ReportOffscreen()
        {
            Fire(new AsteroidViewOffscreen(ViewId, _size));
        }

        public void ReportDestroyedByHit()
        {
            Fire(new AsteroidDestroyed(ViewId, _size, Motor.Position, Motor.Velocity, transform.localScale));
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
        }

        public class Pool : ViewPool<AsteroidSpawnCommand, AsteroidView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(AsteroidSpawnCommand args, AsteroidView item)
            {
                item.Reinitialize(args);
                base.Reinitialize(args, item);
            }
        }
    }
}