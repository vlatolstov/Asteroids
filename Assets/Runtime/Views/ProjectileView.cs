using System;
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
        private float _life;
        private Pool _pool;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _sr = GetComponent<SpriteRenderer>();

            GetComponent<Collider2D>().isTrigger = true;
        }


        void FixedUpdate()
        {
            _life -= Time.fixedDeltaTime;
            if (_life <= 0f)
            {
                _pool.Despawn(this);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            _pool.Despawn(this);
        }

        public void Reinitialize(Pool pool, ProjectileShoot shootData)
        {
            _pool = pool;

            var conf = shootData.Projectile;
            
            transform.position = shootData.Position;
            
            transform.localScale = new Vector3(conf.Size, conf.Size);
            
            var projectileVelocity = conf.Speed * shootData.Direction;
            _rb.linearVelocity = shootData.InheritVelocity + projectileVelocity;
            
            float lifeTime = conf.Lifetime;
            _life = lifeTime > 0 ? lifeTime : _defaultLife;
            
            _sr.sprite = conf.Sprite;
            
            gameObject.layer = shootData.Layer;
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