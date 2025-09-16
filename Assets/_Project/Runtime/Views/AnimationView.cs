using _Project.Runtime.Abstract.MVP;
using UnityEngine;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(Animator))]
    public class AnimationView : BaseView
    {
        private Pool _pool;
        private Animator _animator;
        private int _clipIndex;
        private float _life;

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

            _pool.Despawn(this);
        }

        private void Reinitialize(RuntimeAnimatorController anim, Vector2 pos, Vector2 scale, Pool pool)
        {
            _pool = pool;
            var ac = _animator.runtimeAnimatorController = anim;
            transform.position = pos;
            transform.localScale = scale;
            _clipIndex = Random.Range(0, ac.animationClips.Length);
            _life = ac.animationClips[_clipIndex].length;
        }

        public class Pool : ViewPool<RuntimeAnimatorController, Vector2, Vector2, AnimationView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(RuntimeAnimatorController anim, Vector2 pos, Vector2 scale,
                AnimationView item)
            {
                base.Reinitialize(anim, pos, scale, item);
                item.Reinitialize(anim, pos, scale, this);
            }
        }
    }
}