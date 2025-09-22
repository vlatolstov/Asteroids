using System;
using System.Collections.Generic;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Weapons;
using UnityEngine;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(SpriteRenderer),
        typeof(Collider2D),
        typeof(Animator))]
    public class AoeAttackView : BaseView
    {
        private SpriteRenderer _sr;
        private Animator _animator;
        private Collider2D _collider;

        private AoeAttackConfig _conf;

        private readonly List<ContactPoint2D> _contactPoints = new();

        private Source _source;
        private float _life;
        private bool _follow;
        private Transform _emitter;
        private float _centerOffset;

        public event Action<AoeHit> AoeHit;
        public event Action<uint> Expired;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;
        }

        private void Update()
        {
            if (_life <= 0 || !_emitter.gameObject.activeSelf)
            {
                Expired?.Invoke(ViewId);
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
                    var target = cont.otherCollider.transform;
                    var hit = new AoeHit(_conf, cont.point, target.rotation, Vector2.one, _source);
                    AoeHit?.Invoke(hit);
                }
            }

            _contactPoints.Clear();
        }
        
        private void Reinitialize(Transform par, AoeWeaponConfig aoe)
        {
            _conf = aoe.Attack;
            _life = _conf.Duration;
            _follow = aoe.Attack.Mode == AoeAttackConfig.AttachMode.FollowEmitter;
            _emitter = par;
            _collider.enabled = true;
            gameObject.layer = par.gameObject.layer;

            _centerOffset = aoe.MuzzleOffset + _conf.Length / 2;
            var worldPos = par.TransformPoint(0f, _centerOffset, 0f);
            transform.SetPositionAndRotation(worldPos, _emitter.rotation);
            
            if (!_conf.AttackAnimation)
            {
                _sr.sprite = _conf.AttackSprite;
            }
            else
            {
                _animator.runtimeAnimatorController = _conf.AttackAnimation;
                _animator.Rebind();
                _animator.Update(0f);
            }

            if (_sr.sprite)
            {
                transform.localScale = new Vector3(_conf.Width, _conf.Length, 1f);
                _sr.size = Vector2.one;
            }
            else
            {
                _sr.transform.localScale = Vector3.one;
            }
            Physics2D.SyncTransforms();
        }

        public class Pool : ViewPool<Transform, AoeWeaponConfig, AoeAttackView>
        {
            public Pool(ViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(Transform par, AoeWeaponConfig aoe, AoeAttackView item)
            {
                base.Reinitialize(par, aoe, item);
                item.Reinitialize(par, aoe);
            }

            protected override void OnDespawned(AoeAttackView item)
            {
                base.OnDespawned(item);
                item._collider.enabled = false;
                item._sr.sprite = null;
                item._animator.runtimeAnimatorController = null;
                item.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                item.transform.localScale = Vector3.one;
            }
        }
    }
}