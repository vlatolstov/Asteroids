using System;
using _Project.Runtime.Abstract.Movement;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Abstract.Weapons;
using _Project.Runtime.Data;
using _Project.Runtime.Movement;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Weapons;
using UnityEngine;

namespace _Project.Runtime.Ship
{
    public class ShipView : BaseMovableView<PlayerMotor>, IFireParamsSource
    {
        [SerializeField]
        private GameObject _mainEngine;

        [SerializeField]
        private GameObject _leftEngine;

        [SerializeField]
        private GameObject _rightEngine;

        private ProjectileWeaponResource _gunResource;
        private ProjectileWeaponData _gunData;
        private ProjectileAttackData _gunAttackData;
        private AoeWeaponResource _aoeWeaponResource;
        private AoeWeaponData _aoeData;
        private AoeAttackData _aoeAttackData;

        private ProjectileWeapon _gun;
        private ProjectileWeaponState _projState;

        private AoeWeapon _aoeWeapon;
        private AoeWeaponState _aoeState;

        private bool _weaponsReady;

        public event Action<ProjectileShot> ProjectileFired;
        public event Action<ProjectileWeaponState> ProjectileWeaponStateChanged;
        public event Action<AoeAttackReleased> AoeAttacked;
        public event Action<AoeWeaponState> AoeWeaponStateChanged;
        public event Action<ShipPose> PoseChanged;
        public event Action<ShipDestroyed> Destroyed;

        private ShipPose _pose;
        private bool _destroyed;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_weaponsReady)
            {
                _gun.FixedTick();
                _aoeWeapon.FixedTick();
            }
        }

        private void Update()
        {
            if (Motor == null || !_weaponsReady)
            {
                return;
            }

            var pose = new ShipPose(Motor.Position, Motor.Velocity, Motor.AngleRadians);
            if (_pose != pose)
            {
                _pose = pose;
                PoseChanged?.Invoke(_pose);
            }

            var projState = _gun.ProvideProjWeaponState();
            if (projState != _projState)
            {
                ProjectileWeaponStateChanged?.Invoke(projState);
            }

            var aoeState = _aoeWeapon.ProvideAoeWeaponState();
            if (aoeState != _aoeState)
            {
                AoeWeaponStateChanged?.Invoke(aoeState);
            }
        }

        private void OnDestroy()
        {
            DisposeWeapons();
        }

        public void Configure(PlayerMotor motor, ProjectileWeaponResource gunResource, ProjectileWeaponData gunData,
            ProjectileAttackData gunAttackData, AoeWeaponResource aoeWeaponResource, AoeWeaponData aoeData,
            AoeAttackData aoeAttackData)
        {
            SetMotor(motor);
            _gunResource = gunResource;
            _gunData = gunData;
            _gunAttackData = gunAttackData;
            _aoeWeaponResource = aoeWeaponResource;
            _aoeData = aoeData;
            _aoeAttackData = aoeAttackData;
            InitializeWeapons();
        }

        private void InitializeWeapons()
        {
            DisposeWeapons();

            if (!_gunResource || !_aoeWeaponResource)
            {
                return;
            }

            _gun = new ProjectileWeapon(_gunResource, _gunData, _gunAttackData, this);
            _aoeWeapon = new AoeWeapon(_aoeWeaponResource, _aoeData, _aoeAttackData, this);

            _gun.ProjectileFired += OnProjectileAttack;
            _aoeWeapon.AttackReleased += OnAoeAttack;
            _weaponsReady = true;
        }

        private void DisposeWeapons()
        {
            if (_gun != null)
            {
                _gun.ProjectileFired -= OnProjectileAttack;
                _gun = null;
            }

            if (_aoeWeapon != null)
            {
                _aoeWeapon.AttackReleased -= OnAoeAttack;
                _aoeWeapon = null;
            }

            _weaponsReady = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != gameObject.layer && !_destroyed)
            {
                Destroyed?.Invoke(new ShipDestroyed(transform.position, transform.rotation, transform.localScale));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.layer != other.gameObject.layer
                && other.CompareTag("Attack")
                && !_destroyed)
            {
                Destroyed?.Invoke(new ShipDestroyed(transform.position, transform.rotation, transform.localScale));
            }
        }

        public void TryShootProjectile()
        {
            if (_weaponsReady)
            {
                _gun.Attack();
            }
        }

        public void TryReleaseAoeAttack()
        {
            if (_weaponsReady)
            {
                _aoeWeapon.Attack();
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
            out int layer, out Source sourceType)
        {
            origin = transform.position;
            direction = transform.up;
            inheritVelocity = Motor?.Velocity ?? Vector2.zero;
            layer = gameObject.layer;
            sourceType = Source.Ship;
            return true;
        }

        private void OnProjectileAttack(ProjectileShot proj)
        {
            ProjectileFired?.Invoke(proj);
        }

        private void OnAoeAttack(AoeAttackReleased aoe)
        {
            AoeAttacked?.Invoke(aoe);
        }

        private void Reinitialize(Vector2 position)
        {
            _destroyed = false;
            Motor?.SetWrapMode(true);

            transform.position = position;
            Motor?.SetPose(position, Vector2.zero, 0f);
        }

        public readonly struct SpawnArgs
        {
            public readonly Vector2 Position;
            public readonly PlayerMotor Motor;
            public readonly ProjectileWeaponResource Gun;
            public readonly ProjectileWeaponData GunData;
            public readonly ProjectileAttackData GunAttackData;
            public readonly AoeWeaponResource Aoe;
            public readonly AoeWeaponData AoeData;
            public readonly AoeAttackData AoeAttackData;

            public SpawnArgs(Vector2 position, PlayerMotor motor, ProjectileWeaponResource gun,
                ProjectileWeaponData gunData, ProjectileAttackData gunAttackData, AoeWeaponResource aoe,
                AoeWeaponData aoeData, AoeAttackData aoeAttackData)
            {
                Position = position;
                Motor = motor;
                Gun = gun;
                GunData = gunData;
                GunAttackData = gunAttackData;
                Aoe = aoe;
                AoeData = aoeData;
                AoeAttackData = aoeAttackData;
            }
        }

        public class Pool : ViewPool<SpawnArgs, ShipView>
        {
            public Pool(Func<ShipView> factory, Transform parent, int warmup)
                : base(factory, parent, warmup)
            { }

            protected override void Reinitialize(SpawnArgs args, ShipView item)
            {
                item.Configure(args.Motor, args.Gun, args.GunData, args.GunAttackData, args.Aoe, args.AoeData,
                    args.AoeAttackData);
                item.Reinitialize(args.Position);
            }

            protected override void OnDespawned(ShipView item)
            {
                item.SetupMainEngine(false);
                item.SetupSideEngines(false, false);

                item.Motor?.SetThrust(0f);
                item.Motor?.SetTurnAxis(0f);

                item._aoeWeapon?.Reinforce();
                item.DisposeWeapons();
                item._pose = default;
                item._destroyed = true;
            }
        }
    }
}
