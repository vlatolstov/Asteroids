using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Settings/Weapons/Projectile")]
    public class ProjectileConfig : ScriptableObject, IProjectileConfig
    {
        [SerializeField]
        [Tooltip("Range of the projectile appearances, randomly picked to spawn")]
        private Sprite[] _sprites;
        
        [SerializeField]
        private Vector2 _size;
        
        [SerializeField]
        private float _speed;
        
        [SerializeField]
        private float _lifetime;
        public Sprite Sprite => _sprites[Random.Range(0, _sprites.Length)];
        public Vector2 Size => _size;
        public float Speed => _speed;
        public float Lifetime => _lifetime;
    }
}