using System;
using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Views
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceView : BaseView
    {
        private Pool _pool;
        private AudioSource _audio;
        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!_audio.isPlaying)
            {
                _pool.Despawn(this);
            }
        }

        private void Reinitialize(Vector2 pos, AudioClip clip, Pool pool)
        {
            _pool = pool;
            
            transform.position = pos;
            _audio.PlayOneShot(clip);
        }

        public class Pool : ViewPool<Vector2, AudioClip, AudioSourceView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(Vector2 par, AudioClip clip, AudioSourceView item)
            {
                base.Reinitialize(par, clip, item);
                item.Reinitialize(par, clip, this);
            }
        }
    }
}