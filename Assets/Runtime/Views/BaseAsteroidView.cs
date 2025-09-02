using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    [RequireComponent(typeof(IMove))]
    public abstract class BaseAsteroidView : BaseView
    {
        public AsteroidId Id { get; private set; }
        public AsteroidSize Size;

        [Inject]
        private IWorldConfig _world;

        private IMove _move;
        private bool _entered;

        private void Awake()
        {
            _move = GetComponent<IMove>();
        }

        void FixedUpdate()
        {
            var inside = _world.ExpandedRect().Contains(_move.Position);
            if (!_entered && inside)
            {
                _entered = true;
            }
            else if (_entered && !inside)
            {
                Emit(new AsteroidViewOffscreen(Id, Size));
            };
        }

        public void ReportDestroyedByHit()
        {
            Emit(new AsteroidViewDestroyed(Id, Size, _move.Position, _move.Velocity));
        }

        public void Reinitialize()
        {
            _move.SetPose(_world.OffscreenPosition, Vector2.zero, 0f);
            _entered = false;
        }
        public void SetId(AsteroidId id) => Id = id;
    }
}