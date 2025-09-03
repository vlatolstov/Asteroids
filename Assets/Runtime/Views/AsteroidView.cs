using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    public class AsteroidView : BaseMovableView
    {
        private AsteroidSize _size;
        private bool _entered;
        
        [Inject]
        private IWorldConfig _world;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y);
            bool inside = _world.ExpandedRect(maxScale / 2 + 1).Contains(Motor.Position);
            
            switch (_entered)
            {
                case false when inside:
                    _entered = true;
                    break;
                case true when !inside:
                    ReportOffscreen();
                    break;
            }
        }

        private void ReportOffscreen()
        {
            Fire(new AsteroidViewOffscreen(ViewId, _size));
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            Fire(new AsteroidViewDestroyed(ViewId, _size, Motor.Position, Motor.Velocity));
        }
        
        public void ReportDestroyedByHit()
        {
            Debug.Log($"Asteroid view {ViewId} reporting destroyed by hit");
            Fire(new AsteroidViewDestroyed(ViewId, _size, Motor.Position, Motor.Velocity));
        }

        private void Reinitialize(AsteroidSpawnRequest args)
        {
            _size = args.Size;
            _entered = false;
            Motor.SetPose(args.Pos, args.Vel, args.AngleRad);

            transform.position = args.Pos;
            transform.localScale = new Vector3(args.Scale, args.Scale);
        }

        public class Pool : ViewPool<AsteroidSpawnRequest, AsteroidView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(AsteroidSpawnRequest args, AsteroidView item)
            {
                item.Reinitialize(args);
                base.Reinitialize(args, item);
            }
        }
    }
}