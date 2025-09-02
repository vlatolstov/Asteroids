using System;
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
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }

        protected virtual void FixedUpdate()
        {
            Motor.MoveRigidbody(_rb);
        }
    }
}