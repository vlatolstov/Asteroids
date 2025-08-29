using Runtime.Abstract.Configs;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "WorldConfig", menuName = "Settings/WorldConfig", order = 0)]
    public class WorldConfig : ScriptableObject, IWorldConfig
    {
        [SerializeField] private Vector2 _worldSize = new(1024, 768);
        public Vector2 WorldSize => _worldSize;
    }
}