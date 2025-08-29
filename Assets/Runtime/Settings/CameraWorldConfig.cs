using System;
using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    public class CameraWorldConfig : IWorldConfig
    {
        private readonly Camera _camera;

        public CameraWorldConfig()
        {
            _camera = Camera.main;

            if (_camera == null)
            {
                throw new Exception("CameraWorldConfig: no Camera found in scene.");
            }

            if (!_camera.orthographic)
            {
                Debug.LogWarning("CameraWorldConfig: camera is not orthographic; size is computed as if orthographic.");
            }
        }

        public Rect WorldRect
        {
            get
            {
                var size = WorldSize;
                Vector2 center = _camera.transform.position;
                return new Rect(center - size /2f, size);
            }
        }

        public Vector2 WorldSize
        {
            get
            {
                float height = 2f * _camera.orthographicSize;
                float width = height * _camera.aspect;
                return new Vector2(width, height);
            }
        }
    }
}