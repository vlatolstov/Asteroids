using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Views
{
    public class ShipView : BaseView, GameControls.IGameplayActions, IPlayerTarget
    {
        private GameControls _controls;
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
            if (_controls == null)
            {
                _controls = new GameControls();
                _controls.Gameplay.SetCallbacks(this);
            }

            _movementData = TryGetComponent(out _movementData) ? _movementData : null;

            _controls.Gameplay.Enable();

            Emit(new ThrustInput(0));
            Emit(new TurnInput(0));
        }

        private void OnDisable()
        {
            _controls?.Gameplay.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_controls == null) return;
            _controls.Gameplay.SetCallbacks(null);
            _controls.Dispose();
            _controls = null;
        }

        public void OnThrust(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                var v = context.ReadValue<float>();
                if (v < 0)
                {
                    v = 0;
                }

                Emit(new ThrustInput(v));
            }
        }

        public void OnTurn(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                var v = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
                Emit(new TurnInput(v));
            }
        }

        public void OnBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Emit(new FireBulletPressed());
            }
        }

        public void OnLaser(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Emit(new FireLaserPressed());
            }
        }

        public class Pool : ViewPool<ShipView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }
        }
    }
}