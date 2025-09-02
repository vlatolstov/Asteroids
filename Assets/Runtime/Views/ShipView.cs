using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Views
{
    public class ShipView : BaseView, IPlayerTarget
    {
        private IMove _movementData;

        [SerializeField]
        private bool _isPlayer;

        public bool IsPlayer => _isPlayer;

        public void FixedUpdate()
        {
            Emit(new ShipPose(_movementData.Position, _movementData.Velocity, _movementData.AngleRadians));
        }

        private void OnEnable()
        {
            _movementData = TryGetComponent(out _movementData) ? _movementData : null;
        }
        
        public class Pool : ViewPool<ShipView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }
        }
    }
}