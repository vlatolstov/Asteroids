using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Views
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class BulletView : BaseView
    {
        [SerializeField]
        private float _defaultLife = 1.5f;

        private Rigidbody2D _rb;
        private float _life;
        private Pool _pool;
        private Faction _faction;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        public void Reinit(Pool pool, Vector2 pos, Vector2 velocity, float lifeTime, Faction faction)
        {
            _pool = pool;
            _faction = faction;

            transform.position = pos;
            _rb.linearVelocity = velocity;
            _life = lifeTime > 0 ? lifeTime : _defaultLife;

            gameObject.SetActive(true);
        }

        void FixedUpdate()
        {
            _life -= Time.fixedDeltaTime;
            if (_life <= 0f)
            {
                _pool.Despawn(this);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_faction == Faction.Enemy)
            {
                Emit(new ShipDestroyed());
            }
            else
            {
                Emit(new ScoreAdded(0));
            }

            _pool.Despawn(this);
        }

        public class Pool : MonoMemoryPool<Vector2, Vector2, float, Faction, BulletView>
        {
            protected override void Reinitialize(Vector2 pos, Vector2 vel, float life, Faction fac, BulletView item)
                => item.Reinit(this, pos, vel, life, fac);

            protected override void OnDespawned(BulletView item)
            {
                item.RemoveAllListeners();
                item._rb.linearVelocity = Vector2.zero;
                item.gameObject.SetActive(false);
                base.OnDespawned(item);
            }
        }
    }
}