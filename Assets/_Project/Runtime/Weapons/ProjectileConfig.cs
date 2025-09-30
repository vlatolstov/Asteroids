using UnityEngine;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Settings/Weapons/Projectile")]
    public class ProjectileConfig : ScriptableObject
    {
        [Header("Logic")]
        [field: SerializeField]
        public Vector2 Size { get; private set; }

        [field: SerializeField]
        public float Speed { get; private set; }

        [field: SerializeField]
        public float Lifetime { get; private set; }

        [Header("Representation")]
        [SerializeField,
         Tooltip("Range of the projectile appearances, randomly picked to spawn")]
        private Sprite[] _sprites;

        public Sprite AttackSprite => _sprites[Random.Range(0, _sprites.Length)];

        [field: SerializeField,
                Tooltip("Range of the projectile hit sounds, randomly picked to play at hit")]
        private AudioClip[] _hitSounds;

        public AudioClip HitSound => _hitSounds[Random.Range(0, _hitSounds.Length)];

        [field: SerializeField]
        public RuntimeAnimatorController HitAnimation { get; private set; }

        [field: SerializeField]
        public RuntimeAnimatorController AttackAnimation { get; private set; }
    }
}