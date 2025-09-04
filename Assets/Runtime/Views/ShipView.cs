using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    public class ShipView : BaseMovableView
    {
        [SerializeField]
        private bool _isPlayer;

        public bool IsPlayer => _isPlayer;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Fire(new ShipPose(Motor.Position, Motor.Velocity, Motor.AngleRadians));
        }

        public class Pool : ViewPool<ShipView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }
        }
    }
}