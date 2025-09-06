using Runtime.Abstract.Visualization;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Settings
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Settings/Weapons/Projectile")]
    public class ProjectileConfig : ScriptableObject, IProjectileConfig, IAttackRepresentation
    {
        [Header("Logic")]
        [SerializeField]
        private Vector2 _size;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _lifetime;

        [Header("Representation")]
        [SerializeField]
        [Tooltip("Range of the projectile appearances, randomly picked to spawn")]
        private Sprite[] _sprites;
        
        [SerializeField]
        [Tooltip("Range of the projectile hit sounds, randomly picked to play at hit")]
        private AudioClip[] _hitSounds;

        public Vector2 Size => _size;
        public float Speed => _speed;
        public float Lifetime => _lifetime;
        public Sprite AttackSprite => _sprites[Random.Range(0, _sprites.Length)];
        public RuntimeAnimatorController AttackAnimation { get; }
        public AudioClip HitSound => _hitSounds[Random.Range(0, _hitSounds.Length)];
    }
}