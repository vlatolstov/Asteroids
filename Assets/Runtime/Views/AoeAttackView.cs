using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Settings;
using Runtime.Utils;
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

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_life <= 0)
            {
                _spriteRenderer.sprite = null;
                _animator.runtimeAnimatorController = null;
                _pool.Despawn(this);
            }

            _life -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Fire(new AoeHit(_conf));
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

            transform.SetParent(par, false);
            gameObject.layer = par.gameObject.layer;

            float distFromParent = aoe.MuzzleOffset + _conf.Length / 2;
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