using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Settings;
using Runtime.Weapons;
using UnityEngine;
using Zenject;
using GM = Runtime.Utils.GeometryMethods;

namespace Runtime.Views
{
    [RequireComponent(typeof(Collider2D),
        typeof(SpriteRenderer))]
    public class UfoView : BaseMovableView, IFireParamsSource
    {
        [Inject]
        private ProjectileWeaponConfig _gunConfig;

        [Inject]
        private IWorldConfig _world;

        [Inject]
        private ChasingEnemyConfig _chase;

        private ProjectileWeapon _gun;

        private float _prevErr;

        private bool _entered;
        private bool _hasTarget;
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
                float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y);
                bool inside = _world.WorldRect.Contains(Motor.Position);

                if (inside)
                {
                    Motor.SetWrapMode(true);
                    _entered = true;
                }

                return;
            }

            if (!_hasTarget)
                return;

            float dt = Time.fixedDeltaTime;

            Vector2 selfPos = Motor.Position;
            Vector2 fwd = GM.AngleToDir(Motor.AngleRadians);

            Vector2 delta = GM.ShortestWrappedDelta(selfPos, _target.Position, _world.WorldRect);
            float dist = delta.magnitude;
            Vector2 aimDir = dist > 1e-5f ? delta / dist : fwd;

            float angErr = GM.SignedAngleRad(fwd, aimDir);
            float dErr = (angErr - _prevErr) / Mathf.Max(dt, 1e-5f);
            _prevErr = angErr;

            float turnAxis = Mathf.Clamp(-(_chase.TurnKp * angErr + _chase.TurnKd * dErr), -1f, 1f);

            float thrust = Mathf.Clamp(_chase.ThrustKp * dist, 0f, _chase.MaxThrust);
            float aimFactor = Mathf.Clamp01(1f - Mathf.Abs(angErr) / (45f * Mathf.Deg2Rad));
            thrust *= Mathf.Lerp(0.35f, 1f, aimFactor);

            Motor.SetThrust(thrust);
            Motor.SetTurnAxis(turnAxis);

            _gun.FixedTick();

            float projSpeed = _gunConfig.Projectile.Speed;
            float tLead = (projSpeed > 0.1f) ? Mathf.Clamp(dist / projSpeed, 0f, _chase.MaxLeadSeconds) : 0f;

            Vector2 leadPoint = _target.Position + _target.Velocity * tLead;
            Vector2 leadDelta = GM.ShortestWrappedDelta(selfPos, leadPoint, _world.WorldRect);
            Vector2 leadDir = leadDelta.sqrMagnitude > 1e-6f ? leadDelta.normalized : aimDir;

            float aimErrDeg = Mathf.Abs(GM.SignedAngleRad(fwd, leadDir)) * Mathf.Rad2Deg;

            if (aimErrDeg <= _chase.AimToleranceDegrees && dist <= _chase.MaxFireDistance)
            {
                _gun.TryAttack();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (gameObject.layer != other.gameObject.layer && !_destroyed)
            {
                _destroyed = true;
                Fire(new UfoDestroyed(ViewId, Motor.Position));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer && !_destroyed)
            {
                _destroyed = true;
                Fire(new UfoDestroyed(ViewId, Motor.Position));
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
            _hasTarget = true;

            // if (_gameState == GameState.Gameplay)
            // {
            //     
            // }
            // else
            // {
            //     _hasTarget = false;
            // }
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