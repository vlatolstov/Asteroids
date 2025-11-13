using _Project.Runtime.Abstract.MVP;
using UnityEngine;

namespace _Project.Runtime.Abstract.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseMovableView<TMotor> : BaseView where TMotor : BaseMotor2D
    {
        private Rigidbody2D _rb;

        public TMotor Motor { get; private set; }
        
        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate()
        {
            if (Motor != null)
            {
                Motor.MoveRigidbody(_rb);
            }
        }

        protected void SetMotor(TMotor motor)
        {
            Motor = motor;
        }

        protected void ApplyAngularVelocity(float angleRadians)
        {
            _rb.angularVelocity = angleRadians * Mathf.Rad2Deg;
        }
    }
}
