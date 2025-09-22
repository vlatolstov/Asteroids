using System;
using _Project.Runtime.Abstract.MVP;
using UnityEngine;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceView : BaseView
    {
        public event Action<uint> Expired;
        private AudioSource _audio;
        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!_audio.isPlaying)
            {
                Expired?.Invoke(ViewId);
            }
        }

        private void Reinitialize(Vector2 pos, AudioClip clip)
        {
            transform.position = pos;
            _audio.PlayOneShot(clip);
        }

        public class Pool : ViewPool<Vector2, AudioClip, AudioSourceView>
        {
            public Pool(ViewsContainer viewsContainer) : base(viewsContainer)
            { }

            protected override void Reinitialize(Vector2 par, AudioClip clip, AudioSourceView item)
            {
                base.Reinitialize(par, clip, item);
                item.Reinitialize(par, clip);
            }
        }
    }
}