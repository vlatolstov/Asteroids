using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using _Project.Runtime.Movement;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Weapons;
using UnityEngine;
using GM = _Project.Runtime.Utils.GeometryMethods;

namespace _Project.Runtime.Ufo
{
    [RequireComponent(typeof(Collider2D),
        typeof(SpriteRenderer))]
    public class UfoView : BaseMovableView<ChasingMotor>, IFireParamsSource
    {
        private ProjectileWeaponResource _gunResource;
        private ProjectileWeaponData _gunData;
        private ProjectileAttackData _gunAttackData;
        private IWorldConfig _world;
        private ChasingUfoData _chase;
        private ProjectileWeapon _gun;

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

        private void OnDestroy()
        {
            if (_gun != null)
            {
                _gun.ProjectileFired -= OnProjectileShot;
                _gun = null;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Motor == null)
            {
                return;
            }

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

            if (_gameState != GameState.Gameplay || !_gunResource)
            {
                return;
            }

            Motor.ChaseTarget(_target);

            _gun?.FixedTick();

            if (_gun != null && CanAttack())
            {
                _gun.Attack();
            }
        }

        public void Configure(ChasingMotor motor, ProjectileWeaponResource gunResource, ProjectileWeaponData gunData,
            ProjectileAttackData gunAttackData, ChasingUfoData chase, IWorldConfig world)
        {
            SetMotor(motor);
            _gunResource = gunResource;
            _gunData = gunData;
            _gunAttackData = gunAttackData;
            _chase = chase;
            _world = world;

            if (_gun != null)
            {
                _gun.ProjectileFired -= OnProjectileShot;
                _gun = null;
            }

            if (_gunResource != null)
            {
                _gun = new ProjectileWeapon(_gunResource, _gunData, _gunAttackData, this);
                _gun.ProjectileFired += OnProjectileShot;
            }
        }

        private bool CanAttack()
        {
            if (Motor == null || _gunResource == null || _gunAttackData == null || _chase == null || _world == null)
            {
                return false;
            }

            var selfPos = Motor.Position;
            var fwd = GM.AngleToDir(Motor.AngleRadians);

            var deltaAim = _target.Position - selfPos;
            float distAim = deltaAim.magnitude;

            float projSpeed = _gunAttackData?.Speed ?? 0f;
            float tLead = projSpeed > 0.1f ? Mathf.Clamp(distAim / projSpeed, 0f, _chase.MaxLeadSeconds) : 0f;

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
                Destroyed?.Invoke(
                    new UfoDestroyed(ViewId, transform.position, transform.rotation, transform.localScale));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer
                && other.CompareTag("Attack")
                && !_destroyed)
            {
                _destroyed = true;
                Destroyed?.Invoke(
                    new UfoDestroyed(ViewId, transform.position, transform.rotation, transform.localScale));
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
            direction = Motor != null ? GM.AngleToDir(Motor.AngleRadians) : Vector2.up;
            inheritVelocity = Motor?.Velocity ?? Vector2.zero;
            layer = gameObject.layer;
            sourceType = Source.Ufo;
            return true;
        }

        private void Reinitialize(in UfoSpawnCommand args)
        {
            Motor?.SetWrapMode(false);
            _entered = false;
            _destroyed = false;
            transform.localScale = new Vector3(args.Scale, args.Scale);
            _selfOffset = Mathf.Max(transform.localScale.x, transform.localScale.y) / 2;
            transform.position = args.Pos;
            Motor?.SetPose(args.Pos, args.Vel, args.AngleRad);

            _sr.sprite = args.Sprite;
        }

        public readonly struct SpawnArgs
        {
            public readonly UfoSpawnCommand Command;
            public readonly ChasingMotor Motor;
            public readonly ProjectileWeaponResource Gun;
            public readonly ProjectileWeaponData GunData;
            public readonly ProjectileAttackData GunAttackData;
            public readonly ChasingUfoData Chase;
            public readonly IWorldConfig World;

            public SpawnArgs(UfoSpawnCommand command, ChasingMotor motor, ProjectileWeaponResource gun,
                ProjectileWeaponData gunData, ProjectileAttackData gunAttackData, ChasingUfoData chase,
                IWorldConfig world)
            {
                Command = command;
                Motor = motor;
                Gun = gun;
                GunData = gunData;
                GunAttackData = gunAttackData;
                Chase = chase;
                World = world;
            }
        }

        public class Pool : ViewPool<SpawnArgs, UfoView>
        {
            public Pool(Func<UfoView> factory, Transform parent, int warmup)
                : base(factory, parent, warmup)
            { }

            protected override void Reinitialize(SpawnArgs args, UfoView item)
            {
                item.Configure(args.Motor, args.Gun, args.GunData, args.GunAttackData, args.Chase, args.World);
                item.Reinitialize(in args.Command);
            }
        }
    }
}
