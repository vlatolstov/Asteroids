using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using UnityEngine;
using Zenject;

namespace Runtime.Abstract.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseMovableView : BaseView
    {
        private Rigidbody2D _rb;

        [Inject]
        public BaseMotor2D<IMovementConfig> Motor;
        
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