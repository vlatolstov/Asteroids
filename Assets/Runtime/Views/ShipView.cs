using System;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Contexts.Global;
using Runtime.Data;
using Runtime.Settings;
using Runtime.Weapons;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    public class ShipView : BaseMovableView, IFireParamsSource
    {
        [SerializeField]
        private GameObject _mainEngine;

        [SerializeField]
        private GameObject _leftEngine;

        [SerializeField]
        private GameObject _rightEngine;

        [Inject]
        private ProjectileWeaponConfig _gunConfig;

        [Inject]
        private AoeWeaponConfig _aoeWeaponConfig;

        public ProjectileWeapon Gun;
        public AoeWeapon AoeWeapon;

        private bool _destroyed;

        protected override void Awake()
        {
            base.Awake();
            Gun = new ProjectileWeapon(_gunConfig, this);
            AoeWeapon = new AoeWeapon(_aoeWeaponConfig, this);
            
            Gun.AttackGenerated += OnWeaponAttack;
            AoeWeapon.AttackGenerated += OnWeaponAttack;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Gun.FixedTick();
            AoeWeapon.FixedTick();
        }

        private void Update()
        {
            Fire(new ShipPose(Motor.Position, Motor.Velocity, Motor.AngleRadians));
            Fire(AoeWeapon.ProvideAoeWeaponState());
        }

        private void OnWeaponAttack(IData attackData)
        {
            switch (attackData)
            {
                case ProjectileShoot p:
                    Fire(p);
                    break;
                case AoeAttackReleased l:
                    Fire(l);
                    break;
                default:
                    Debug.LogWarning($"Unknown attack data: {attackData?.GetType().Name}");
                    break;
            }
        }

        public void SetupMainEngine(bool main)
        {
            _mainEngine.SetActive(main);
        }

        public void SetupSideEngines(bool left, bool right)
        {
            _leftEngine.SetActive(left);
            _rightEngine.SetActive(right);
        }
        
        public bool TryGetFireParams(out Vector2 origin, out Vector2 direction, out Vector2 inheritVelocity,
            out int layer)
        {
            origin = transform.position;
            direction = transform.up;
            inheritVelocity = Motor?.Velocity ?? Vector2.zero;
            layer = gameObject.layer;
            return true;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (gameObject.layer != other.gameObject.layer && !_destroyed)
            {
                _destroyed = true;
                Fire(new ShipDestroyed(ViewId, Motor.Position));
            }
        }

        private void TurnSystemsOff()
        {
            AoeWeapon.Reinforce();
            _destroyed = false;
            Motor.SetThrust(0f);
            Motor.SetTurnAxis(0f);
            Motor.SetWrapMode(true);
        }
        
        private void Reinitialize(Vector2 position)
        {
            TurnSystemsOff();
            
            transform.position = position;
            Motor.SetPose(position, Vector2.zero, 0f);
            
            Fire(new ShipSpawned(true, ViewId, Motor.Position));
        }

        public class Pool : ViewPool<Vector2, ShipView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(Vector2 p1, ShipView item)
            {
                base.Reinitialize(p1, item);
                item.Reinitialize(p1);
            }
        }
    }
}