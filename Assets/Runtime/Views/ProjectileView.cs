using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
    public sealed class ProjectileView : BaseView
    {
        [SerializeField]
        private float _defaultLife = 1f;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Pool _pool;
        private float _life;
        private bool _spawned;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_spawned)
            {
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

            var conf = shootData.Projectile;

            transform.position = shootData.Position;
            transform.localScale = conf.Size;

            var dir = shootData.Direction;
            var projectileVelocity = conf.Speed * dir;

            transform.up = dir;

            _rb.linearVelocity = shootData.InheritVelocity + projectileVelocity;

            float lifeTime = conf.Lifetime;
            _life = lifeTime > 0 ? lifeTime : _defaultLife;

            _sr.sprite = conf.Sprite;

            gameObject.layer = shootData.Layer;
            _spawned = true;
            gameObject.SetActive(true);
        }

        public class Pool : MonoMemoryPool<ProjectileShoot, ProjectileView>
        {
            protected override void Reinitialize(ProjectileShoot shootData, ProjectileView item)
                => item.Reinitialize(this, shootData);

            protected override void OnDespawned(ProjectileView item)
            {
                item._rb.linearVelocity = Vector2.zero;
                item.gameObject.SetActive(false);
                base.OnDespawned(item);
            }
        }
    }
}