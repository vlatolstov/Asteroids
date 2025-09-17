using System;
using UnityEngine;

namespace _Project.Runtime.Models
{
    public class InputModel
    {
        private float _thrustInput;
        private float _turnInput;

        public event Action<float> ThrustChanged;
        public event Action<float> TurnChanged;
        public event Action FireGunPressed;
        public event Action AoeAttackPressed;

        public void ChangeThrustInput(float input)
        {
            _thrustInput = Mathf.Clamp(input, 0f, 1f);;
            ThrustChanged?.Invoke(_thrustInput);
        }

        public void ChangeTurnInput(float input)
        {
            _turnInput = Mathf.Clamp(input, -1f, 1f);
            TurnChanged?.Invoke(_turnInput);
        }

        public void PressFireGun()
        {
            FireGunPressed?.Invoke();
        }

        public void PressAoeAttack()
        {
            AoeAttackPressed?.Invoke();
        }
    }
}