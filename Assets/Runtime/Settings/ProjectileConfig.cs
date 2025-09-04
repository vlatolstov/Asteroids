using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Settings/Weapons/Projectile")]
    public class ProjectileConfig : ScriptableObject, IProjectileConfig
    {
        [SerializeField]
        private Sprite _sprite;
        
        [SerializeField]
        private float _size;
        
        [SerializeField]
        private float _speed;
        
        [SerializeField]
        private float _lifetime;
        public Sprite Sprite => _sprite;
        public float Size => _size;
        public float Speed => _speed;
        public float Lifetime => _lifetime;
    }
}