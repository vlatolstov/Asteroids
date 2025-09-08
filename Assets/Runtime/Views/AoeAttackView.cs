using System.Collections.Generic;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Utils;
using Runtime.Weapons;
using UnityEngine;

namespace Runtime.Views
{
    [RequireComponent(typeof(SpriteRenderer),
        typeof(Collider2D),
        typeof(Animator))]
    public class AoeAttackView : BaseView
    {
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Collider2D _collider;

        private AoeAttackConfig _conf;

        private float _life;
        private Pool _pool;

        private readonly List<ContactPoint2D> _contactPoints = new();
        
        private bool _follow;
        private Transform _emitter;
        private float _centerOffset;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;
        }

        private void Update()
        {
            if (_life <= 0)
            {
                TurnOff();
            }

            _life -= Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (_follow && _emitter)
            {
                var worldPos = _emitter.TransformPoint(0f, _centerOffset, 0f);
                transform.SetPositionAndRotation(worldPos, _emitter.rotation);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetContacts(_contactPoints) > 0)
            {
                foreach (var cont in _contactPoints)
                {
                    Fire(new AoeHit(_conf, cont.point));
                }
            }

            _contactPoints.Clear();
        }
        
        public void TurnOff()
        {
            _collider.enabled = false;
            _spriteRenderer.sprite = null;
            _animator.runtimeAnimatorController = null;
            transform.SetParent(null, false);
            _pool.Despawn(this);
        }

        private void Reinitialize(Transform par, AoeWeaponConfig aoe, Pool pool)
        {
            _pool = pool;
            _conf = aoe.Attack;
            _life = _conf.Duration;

            if (!_conf.AttackAnimation)
            {
                _spriteRenderer.sprite = _conf.AttackSprite;
            }
            else
            {
                _animator.runtimeAnimatorController = _conf.AttackAnimation;
            }

            _emitter = par;
            _centerOffset = aoe.MuzzleOffset + _conf.Length / 2;
            
            var worldPos = _emitter.TransformPoint(0f, _centerOffset, 0f);
            
            transform.SetPositionAndRotation(worldPos, _emitter.rotation);
            GeometryMethods.SetWorldSizeOfChildObject(_spriteRenderer, _conf.Width, _conf.Length);
            gameObject.layer = par.gameObject.layer;
            
            _collider.enabled = true;
            Physics2D.SyncTransforms();

            _follow = aoe.Attack.Mode == AoeAttachMode.FollowEmitter;
        }

        public class Pool : ViewPool<Transform, AoeWeaponConfig, AoeAttackView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(Transform par, AoeWeaponConfig aoe, AoeAttackView item)
            {
                base.Reinitialize(par, aoe, item);
                item.Reinitialize(par, aoe, this);
            }
        }
    }
}