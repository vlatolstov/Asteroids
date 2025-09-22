using System;
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

        private Source _source;
        private ProjectileConfig _conf;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private float _life;
        private bool _spawned;
        
        public event Action<uint, ProjectileHit> ProjectileHit;
        public event Action<uint> Expired;

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
                Expired?.Invoke(ViewId);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_spawned)
            {
                var hit = new ProjectileHit(_conf, transform.position, transform.rotation, transform.localScale, _source);
                ProjectileHit?.Invoke(ViewId, hit);
            }
        }

        public void Reinitialize(ProjectileShot shotData)
        {
            _conf = shotData.Weapon.Projectile;

            transform.position = shotData.Position;
            transform.localScale = _conf.Size;

            var dir = shotData.Direction;
            var projectileVelocity = _conf.Speed * dir;

            transform.up = dir;

            _rb.linearVelocity = shotData.InheritVelocity + projectileVelocity;

            float lifeTime = _conf.Lifetime;
            _life = lifeTime > 0 ? lifeTime : _defaultLife;

            _sr.sprite = _conf.AttackSprite;

            gameObject.layer = shotData.Layer;
            _source = shotData.Source;
            _spawned = true;
            gameObject.SetActive(true);
        }

        public class Pool : ViewPool<ProjectileShot, ProjectileView>
        {
            public Pool(ViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(ProjectileShot shotData, ProjectileView item)
                => item.Reinitialize(shotData);
        }
    }
}