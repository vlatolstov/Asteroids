using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Views
{
    //TODO separate movement and view logic
    public class ShipView : BaseView, GameControls.IGameplayActions, IPlayerTarget
    {
        private GameControls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new GameControls();
                _controls.Gameplay.SetCallbacks(this);
            }

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