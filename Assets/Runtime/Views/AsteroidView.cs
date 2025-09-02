using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    //TODO сделать один общий тип и один общий пул, сетапить скейл в зависимости от AsteroidConfig и Scale запроса
    [RequireComponent(typeof(IMove))]
    public class AsteroidView : BaseView
    {
        private AsteroidSize _size;

        [Inject]
        private IWorldConfig _world;

        private IMove _move;
        private bool _entered;

        private void Awake()
        {
            _move = GetComponent<IMove>();
        }

        private void FixedUpdate()
        {
            var maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y);
            var inside = _world.ExpandedRect(maxScale / 2 + 1).Contains(_move.Position);
            if (!_entered && inside)
            {
                _entered = true;
            }
            else if (_entered && !inside)
            {
                ReportOffscreen();
            }
        }

        private void ReportOffscreen()
        {
            Emit(new AsteroidViewOffscreen(ViewId, _size));
        }

        public void ReportDestroyedByHit()
        {
            Emit(new AsteroidViewDestroyed(ViewId, _size, _move.Position, _move.Velocity));
        }

        private void Reinitialize(AsteroidSpawnRequest args)
        {
            _size = args.Size;
            _move.SetPose(args.Pos, args.Vel, args.AngleRad);
            _entered = false;

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