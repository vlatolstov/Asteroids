using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Views
{
    public class InputReceiverView : BaseView, GameControls.IGameplayActions
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
        }

        private void OnDisable()
        {
            _controls?.Gameplay.Disable();
        }

        protected virtual void OnDestroy()
        {
            if (_controls == null)
            {
                return;
            }

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

                Fire(new ThrustInput(v));
            }
        }

        public void OnTurn(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                var v = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
                Fire(new TurnInput(v));
            }
        }

        public void OnBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Fire(new FireBulletPressed());
            }
        }

        public void OnLaser(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Fire(new FireLaserPressed());
            }
        }
    }
}