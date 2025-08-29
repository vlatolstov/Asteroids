using Runtime.Abstract.Configs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "WorldConfig", menuName = "Settings/WorldConfig", order = 0)]
    public class CustomWorldConfig : ScriptableObject, IWorldConfig
    {
        [FormerlySerializedAs("_worldSize")]
        [SerializeField]
        private Vector2 _size = new(1024, 768);

        [SerializeField]
        private Vector2 _center = new(0, 0);

        public Rect WorldRect => new(_center - _size / 2f, _size);

        public Vector2 WorldSize => _size;
    }
}