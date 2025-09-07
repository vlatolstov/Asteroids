using System.Collections.Generic;
using Runtime.Abstract.MVP;
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
        private AoeAttackConfig _conf;
        
        private float _life;
        private Pool _pool;

        private readonly List<ContactPoint2D> _contactPoints = new();

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_life <= 0)
            {
                TurnOff();
            }

            _life -= Time.deltaTime;
        }

        public void TurnOff()
        {
            _spriteRenderer.sprite = null;
            _animator.runtimeAnimatorController = null;
            _pool.Despawn(this);
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

        private void Reinitialize(Transform par, AoeWeaponConfig aoe, Pool pool)
        {
            _pool = pool;
            _conf = aoe.Attack;
            _life = _conf.Duration;

            AttachToTransform(par, aoe.MuzzleOffset);
        }

        private void AttachToTransform(Transform par, float muzzleOffset = 0f)
        {
            if (!_conf.AttackAnimation)
            {
                _spriteRenderer.sprite = _conf.AttackSprite;
            }
            else
            {
                _animator.runtimeAnimatorController = _conf.AttackAnimation;
            }

            transform.SetParent(par, false);
            gameObject.layer = par.gameObject.layer;

            float distFromParent = muzzleOffset + _conf.Length / 2;
            transform.localPosition = new Vector3(0f, distFromParent / par.lossyScale.y, 0f);
            
            GeometryMethods.SetWorldSizeOfChildObject(_spriteRenderer, _conf.Width, _conf.Length);
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