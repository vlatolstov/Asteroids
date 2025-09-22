using System;
using _Project.Runtime.Abstract.MVP;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(Animator))]
    public class AnimationView : BaseView
    {
        private Animator _animator;
        private int _clipIndex;
        private float _life;

        public event Action<uint> Expired;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_life > 0)
            {
                _life -= Time.deltaTime;
                return;
            }

            Expired?.Invoke(ViewId);
        }

        private void Reinitialize(RuntimeAnimatorController anim, Vector2 pos, Quaternion rotation, Vector2 scale)
        {
            var ac = _animator.runtimeAnimatorController = anim;
            transform.position = pos;
            transform.rotation = rotation;
            transform.localScale = scale;
            _clipIndex = Random.Range(0, ac.animationClips.Length);
            _life = ac.animationClips[_clipIndex].length;
        }

        public class Pool : ViewPool<RuntimeAnimatorController, Vector2, Quaternion, Vector2, AnimationView>
        {
            public Pool(ViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(RuntimeAnimatorController anim, Vector2 pos, Quaternion rotation,
                Vector2 scale,
                AnimationView item)
            {
                base.Reinitialize(anim, pos, rotation, scale, item);
                item.Reinitialize(anim, pos, rotation, scale);
            }
        }
    }
}