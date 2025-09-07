using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Movement;
using Runtime.Settings;
using Runtime.Weapons;
using UnityEngine;
using Zenject;
using GM = Runtime.Utils.GeometryMethods;

namespace Runtime.Views
{
    [RequireComponent(typeof(Collider2D),
        typeof(SpriteRenderer))]
    public class UfoView : BaseMovableView<ChasingMotor>, IFireParamsSource
    {
        [Inject]
        private ProjectileWeaponConfig _gunConfig;

        [Inject]
        private IWorldConfig _world;

        [Inject]
        private ChasingEnemyConfig _chase;

        private ProjectileWeapon _gun;

        private bool _entered;
        
        private ShipPose _target;
        private GameState _gameState;

        private SpriteRenderer _sr;
        private bool _destroyed;

        protected override void Awake()
        {
            base.Awake();
            _sr = GetComponent<SpriteRenderer>();
            _gun = new ProjectileWeapon(_gunConfig, this);

            _gun.AttackGenerated += OnWeaponAttack;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_entered)
            {
                bool inside = Motor.IsInsideWorldRect();

                if (inside)
                {
                    Motor.SetWrapMode(true);
                    _entered = true;
                }

                return;
            }

            if (_gameState != GameState.Gameplay)
            {
                return;
            }

            Motor.ChaseTarget(_target);
            
            _gun.FixedTick();

            if (CanAttack())
            {
                _gun.TryAttack();
            }
        }

        private bool CanAttack()
        {
            var selfPos = Motor.Position;
            var fwd = GM.AngleToDir(Motor.AngleRadians);

            var deltaAim = _target.Position - selfPos;
            float distAim = deltaAim.magnitude;

            float projSpeed = _gunConfig.Projectile.Speed;
            float tLead = (projSpeed > 0.1f) ? Mathf.Clamp(distAim / projSpeed, 0f, _chase.MaxLeadSeconds) : 0f;

            var leadPoint = _target.Position + _target.Velocity * tLead;
            leadPoint = GM.ClampPointToRect(leadPoint, _world.WorldRect);

            var leadDir = (leadPoint - selfPos);
            float leadDist = leadDir.magnitude;
            if (leadDist > 1e-5f) leadDir /= leadDist;
            else leadDir = fwd;

            float aimErrDeg = Mathf.Abs(GM.SignedAngleRad(fwd, leadDir)) * Mathf.Rad2Deg;

            return aimErrDeg <= _chase.AimToleranceDegrees && distAim <= _chase.MaxFireDistance;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (gameObject.layer != other.gameObject.layer && !_destroyed)
            {
                _destroyed = true;
                Fire(new UfoDestroyed(ViewId, Motor.Position, transform.localScale));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer 
                && other.CompareTag("Attack")
                && !_destroyed)
            {
                _destroyed = true;
                Fire(new UfoDestroyed(ViewId, Motor.Position, transform.localScale));
            }
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

        public void UpdateShipPose(in ShipPose shipPose)
        {
            _target = shipPose;
        }

        public void UpdateGameState(GameState gameState)
        {
            _gameState = gameState;
        }

        public bool TryGetFireParams(out Vector2 origin, out Vector2 direction, out Vector2 inheritVelocity,
            out int layer)
        {
            origin = transform.position;
            direction = GM.AngleToDir(Motor.AngleRadians);
            inheritVelocity = Motor.Velocity;
            layer = gameObject.layer;
            return true;
        }

        private void Reinitialize(in UfoSpawnCommand args)
        {
            Motor.SetWrapMode(false);
            _entered = false;
            _destroyed = false;
            transform.localScale = new Vector3(args.Scale, args.Scale);
            transform.position = args.Pos;
            Motor.SetPose(args.Pos, args.Vel, args.AngleRad);

            _sr.sprite = args.Sprite;

            Fire(new UfoSpawned(ViewId, transform.position));
        }

        public class Pool : ViewPool<UfoSpawnCommand, UfoView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(UfoSpawnCommand args, UfoView item)
            {
                base.Reinitialize(args, item);
                item.Reinitialize(in args);
            }
        }
    }
}