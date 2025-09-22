using System;
using UnityEngine;

namespace _Project.Runtime.Models
{
    public class InputModel
    {
        public event Action<float> ThrustChanged;
        public event Action<float> TurnChanged;
        public event Action FireGunPressed;
        public event Action AoeAttackPressed;

        public void ChangeThrustInput(float input)
        {
            float clampedInput = Mathf.Clamp(input, 0f, 1f);
            ThrustChanged?.Invoke(clampedInput);
        }

        public void ChangeTurnInput(float input)
        {
            float clampedInput = Mathf.Clamp(input, -1f, 1f);
            TurnChanged?.Invoke(clampedInput);
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