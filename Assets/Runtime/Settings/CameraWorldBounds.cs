using System;
using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [RequireComponent(typeof(Camera))]
    public class CameraWorldBounds : MonoBehaviour, IWorldConfig
    {
        [SerializeField]
        private float _wrapOffset = 0.5f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            if (!_camera)
            {
                throw new MissingComponentException("Missing camera component");
            }
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

        public float WrapOffset => Mathf.Max(0f, _wrapOffset);
        public Vector2 OffscreenPosition => new(WorldRect.min.x - 10f, WorldRect.min.y - 10f);
    }
}