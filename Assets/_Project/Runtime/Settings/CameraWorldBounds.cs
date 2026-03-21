using _Project.Runtime.Abstract.Configs;
using UnityEngine;

namespace _Project.Runtime.Settings
{
    public class CameraWorldBounds : IWorldConfig
    {
        private readonly Camera _camera;

        public CameraWorldBounds()
        {
            _camera = Camera.main;
        }
        
        public Rect WorldRect
        {
            get
            {
                float h = _camera.orthographicSize * 2f;
                float w = h * _camera.aspect;
                Vector2 center = _camera.transform.position;
                return new Rect(center - new Vector2(w, h) * 0.5f, new Vector2(w, h));
            }
        }

        public float WrapOffset => 0.5f;
        public Vector2 OffscreenPosition => new(WorldRect.min.x - 10f, WorldRect.min.y - 10f);
    }
}