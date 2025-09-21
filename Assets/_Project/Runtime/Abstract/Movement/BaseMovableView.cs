using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Movement;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Abstract.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseMovableView<TMotor> : BaseView where TMotor : BaseMotor2D<MovementConfig>
    {
        private Rigidbody2D _rb;

        [Inject]
        public TMotor Motor;
        
        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate()
        {
            Motor.MoveRigidbody(_rb);
        }
        
        public void ApplyAngularVelocity(float angleRadians)
        {
            _rb.angularVelocity = angleRadians * Mathf.Rad2Deg;
        }
    }
}