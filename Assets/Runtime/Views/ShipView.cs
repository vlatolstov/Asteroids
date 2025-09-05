using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Contexts.Global;
using Runtime.Data;
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

        [Inject(Id = WeaponsInstaller.WeaponId.ShipGun)]
        private IProjectileWeaponConfig _gunConfig;

        public ProjectileWeapon Gun;

        private bool _destriyed;

        protected override void Awake()
        {
            base.Awake();
            Gun = new ProjectileWeapon(_gunConfig, this);
            Gun.AttackGenerated += OnWeaponAttack;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Fire(new ShipPose(Motor.Position, Motor.Velocity, Motor.AngleRadians));

            Gun.FixedTick();
        }

        private void OnWeaponAttack(IData attackData)
        {
            switch (attackData)
            {
                case ProjectileShoot p:
                    Fire(p);
                    break;
                case AoeAttack l:
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
            if (gameObject.layer != other.gameObject.layer && !_destriyed)
            {
                _destriyed = true;
                Fire(new ShipDestroyed(ViewId, Motor.Position));
            }
        }

        private void Reinitialize(Vector2 position)
        {
            
            _destriyed = false;
            transform.position = position;
            Motor.SetPose(position, Vector2.zero, 0f);
            Fire(new ShipSpawned(ViewId, Motor.Position));
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