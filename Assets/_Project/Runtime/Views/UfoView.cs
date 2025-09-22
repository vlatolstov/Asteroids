using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using _Project.Runtime.Movement;
using _Project.Runtime.Settings;
using _Project.Runtime.Weapons;
using UnityEngine;
using Zenject;
using GM = _Project.Runtime.Utils.GeometryMethods;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(Collider2D),
        typeof(SpriteRenderer))]
    public class UfoView : BaseMovableView<ChasingMotor>, IFireParamsSource
    {
        private ProjectileWeaponConfig _gunConfig;
        private IWorldConfig _world;
        private ChasingEnemyConfig _chase;
        private ProjectileWeapon _gun;

        [Inject]
        private void Construct(ProjectileWeaponConfig gunConfig, IWorldConfig world, ChasingEnemyConfig chase)
        {
            _gunConfig = gunConfig;
            _world = world;
            _chase = chase;

            _gun = new ProjectileWeapon(_gunConfig, this);
            _gun.ProjectileFired += OnProjectileShot;
        }

        private ShipPose _target;
        private GameState _gameState;
        private bool _entered;
        private bool _destroyed;
        private float _selfOffset;

        private SpriteRenderer _sr;

        public event Action<ProjectileShot> ProjectileFired;
        public event Action<UfoDestroyed> Destroyed;
        public event Action<UfoOffscreen> Offscreen;

        protected override void Awake()
        {
            base.Awake();
            _sr = GetComponent<SpriteRenderer>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            bool inside = Motor.IsInsideWorldRect(_selfOffset);
            switch (_entered)
            {
                case false when inside:
                    Motor.SetWrapMode(true);
                    _entered = true;
                    break;
                case true when !inside:
                    Offscreen?.Invoke(new UfoOffscreen(ViewId));
                    break;
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
                Destroyed?.Invoke(new UfoDestroyed(ViewId, transform.position, transform.rotation, transform.localScale));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer
                && other.CompareTag("Attack")
                && !_destroyed)
            {
                _destroyed = true;
                Destroyed?.Invoke(new UfoDestroyed(ViewId, transform.position, transform.rotation, transform.localScale));
            }
        }

        private void OnProjectileShot(ProjectileShot shot)
        {
            ProjectileFired?.Invoke(shot);
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
            out int layer, out Source sourceType)
        {
            origin = transform.position;
            direction = GM.AngleToDir(Motor.AngleRadians);
            inheritVelocity = Motor.Velocity;
            layer = gameObject.layer;
            sourceType = Source.Ufo;
            return true;
        }

        private void Reinitialize(in UfoSpawnCommand args)
        {
            Motor.SetWrapMode(false);
            _entered = false;
            _destroyed = false;
            transform.localScale = new Vector3(args.Scale, args.Scale);
            _selfOffset = Mathf.Max(transform.localScale.x, transform.localScale.y) / 2;
            transform.position = args.Pos;
            Motor.SetPose(args.Pos, args.Vel, args.AngleRad);

            _sr.sprite = args.Sprite;
        }

        public class Pool : ViewPool<UfoSpawnCommand, UfoView>
        {
            public Pool(ViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(UfoSpawnCommand args, UfoView item)
            {
                base.Reinitialize(args, item);
                item.Reinitialize(in args);
            }
        }
    }
}