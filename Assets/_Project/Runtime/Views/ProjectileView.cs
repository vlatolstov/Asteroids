using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Weapons;
using UnityEngine;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(Rigidbody2D), 
        typeof(Collider2D), 
        typeof(SpriteRenderer))]
    public sealed class ProjectileView : BaseView
    {
        [SerializeField]
        private float _defaultLife = 1f;

        private ProjectileConfig _conf;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Pool _pool;
        private float _life;
        private bool _spawned;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }


        void FixedUpdate()
        {
            _life -= Time.fixedDeltaTime;
            if (_life <= 0f && _spawned)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_spawned)
            {
                Fire(new ProjectileHit(_conf, transform.position));
                Despawn();
            }
            
        }

        private void Despawn()
        {
            _spawned = false;
            _pool.Despawn(this);
        }

        public void Reinitialize(Pool pool, ProjectileShoot shootData)
        {
            _pool = pool;

            _conf = shootData.Weapon.Projectile;

            transform.position = shootData.Position;
            transform.localScale = _conf.Size;

            var dir = shootData.Direction;
            var projectileVelocity = _conf.Speed * dir;

            transform.up = dir;

            _rb.linearVelocity = shootData.InheritVelocity + projectileVelocity;

            float lifeTime = _conf.Lifetime;
            _life = lifeTime > 0 ? lifeTime : _defaultLife;

            _sr.sprite = _conf.AttackSprite;

            gameObject.layer = shootData.Layer;
            _spawned = true;
            gameObject.SetActive(true);
        }

        public class Pool : ViewPool<ProjectileShoot, ProjectileView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(ProjectileShoot shootData, ProjectileView item)
                => item.Reinitialize(this, shootData);
        }
    }
}