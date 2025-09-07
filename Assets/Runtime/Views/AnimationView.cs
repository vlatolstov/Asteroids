using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Views
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

        private void Reinitialize(RuntimeAnimatorController anim, Vector2 pos, Pool pool)
        {
            _pool = pool;
            var ac = _animator.runtimeAnimatorController = anim;
            transform.position = pos;
            _clipIndex = Random.Range(0, ac.animationClips.Length);
            _life = ac.animationClips[_clipIndex].length;
        }

        public class Pool : ViewPool<RuntimeAnimatorController, Vector2, AnimationView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(RuntimeAnimatorController anim, Vector2 pos,
                AnimationView item)
            {
                base.Reinitialize(anim, pos, item);
                item.Reinitialize(anim, pos, this);
            }
        }
    }
}