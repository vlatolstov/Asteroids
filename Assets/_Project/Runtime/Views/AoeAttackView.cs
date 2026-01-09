using System;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.RemoteConfig;
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

        private AoeAttackResource _attackResource;
        private AoeAttackData _attackData;
        private AoeWeaponData _weaponData;

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
            var target = other.transform;
            var hit = new AoeHit(_attackResource, _attackData, target.position, target.rotation, Vector2.one, _source);
            AoeHit?.Invoke(hit);
        }

        private void Reinitialize(AoeAttackReleased attack)
        {
            _attackResource = attack.Weapon.Attack;
            _attackData = attack.AttackData ?? new AoeAttackData();
            _weaponData = attack.WeaponData ?? new AoeWeaponData();
            _life = _attackData.Duration;
            _follow = _attackData.AttachMode == (int)AoeAttackResource.AttachMode.FollowEmitter;
            _emitter = attack.Emitter;
            _collider.enabled = true;
            _source = attack.Source;
            gameObject.layer = attack.Emitter.gameObject.layer;

            _centerOffset = _weaponData.MuzzleOffset + _attackData.Length / 2f;
            var worldPos = attack.Emitter.TransformPoint(0f, _centerOffset, 0f);
            transform.SetPositionAndRotation(worldPos, _emitter.rotation);

            if (!_attackResource.AttackAnimation)
            {
                _sr.sprite = _attackResource.AttackSprite;
            }
            else
            {
                _animator.runtimeAnimatorController = _attackResource.AttackAnimation;
                _animator.Rebind();
                _animator.Update(0f);
            }

            if (_sr.sprite)
            {
                transform.localScale = new Vector3(_attackData.Width, _attackData.Length, 1f);
                _sr.size = Vector2.one;
            }
            else
            {
                _sr.transform.localScale = Vector3.one;
            }

            Physics2D.SyncTransforms();
        }

        public class Pool : ViewPool<AoeAttackReleased, AoeAttackView>
        {
            public Pool(Func<AoeAttackView> factory, Transform parent, int warmup)
                : base(factory, parent, warmup)
            {
            }

            protected override void Reinitialize(AoeAttackReleased attack, AoeAttackView item)
            {
                item.Reinitialize(attack);
            }

            protected override void OnDespawned(AoeAttackView item)
            {
                item._source = Source.Undefined;
                item._collider.enabled = false;
                item._sr.sprite = null;
                item._animator.runtimeAnimatorController = null;
                item.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                item.transform.localScale = Vector3.one;
            }
        }
    }
}
