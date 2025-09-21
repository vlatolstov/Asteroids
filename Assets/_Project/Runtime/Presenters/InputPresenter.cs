using System;
using _Project.Runtime.Models;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class InputPresenter : GameControls.IGameplayActions, IFixedTickable, IDisposable
    {
        private readonly InputModel _inputModel;
        private GameControls _controls;
        private bool _isFireGunButtonHeld;
        private bool _isAoeAttackButtonHeld;

        public InputPresenter(InputModel inputModel)
        {
            _inputModel = inputModel;

            _controls = new GameControls();
            _controls.Gameplay.SetCallbacks(this);
            _controls.Gameplay.Enable();
        }

        public void FixedTick()
        {
            if (_isFireGunButtonHeld)
            {
                _inputModel.PressFireGun();
            }

            if (_isAoeAttackButtonHeld)
            {
                _inputModel.PressAoeAttack();
            }
        }

        public void Dispose()
        {
            if (_controls == null)
            {
                return;
            }

            _controls.Gameplay.Disable();
            _controls.Gameplay.SetCallbacks(null);
            _controls.Dispose();
            _controls = null;
        }

        public void OnThrust(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                _inputModel.ChangeThrustInput(context.ReadValue<float>());
            }
        }

        public void OnTurn(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                _inputModel.ChangeTurnInput(context.ReadValue<float>());
            }
        }

        public void OnBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isFireGunButtonHeld = true;
            }

            if (context.canceled)
            {
                _isFireGunButtonHeld = false;
            }
        }

        public void OnLaser(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isAoeAttackButtonHeld = true;
            }

            if (context.canceled)
            {
                _isAoeAttackButtonHeld = false;
            }
        }
    }
}