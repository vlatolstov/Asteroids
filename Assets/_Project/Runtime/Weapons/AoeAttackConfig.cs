using _Project.Runtime.Abstract.Visualization;
using _Project.Runtime.Abstract.Weapons;
using UnityEngine;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "AoeAttackConfig", menuName = "Settings/Weapons/AOE Attack", order = 0)]
    public class AoeAttackConfig : ScriptableObject, IAoeAttackConfig, IAttackRepresentation
    {
        [Header("Logic")]
        [SerializeField]
        private float _length = 2f;

        [SerializeField]
        private float _width = 0.5f;

        [SerializeField]
        private float _duration = 0.5f;

        [SerializeField]
        private AoeAttachMode _mode;

        [Header("Representation")]
        [SerializeField]
        [Tooltip("Range of the projectile appearances, randomly picked to spawn")]
        private Sprite[] _sprites;

        [SerializeField]
        [Tooltip("Range of the projectile hit sounds, randomly picked to play at hit")]
        private AudioClip[] _hitSounds;

        [SerializeField]
        private RuntimeAnimatorController _attackAnimation;


        public float Length => _length;
        public float Width => _width;
        public float Duration => _duration;

        public AoeAttachMode Mode => _mode;

        public Sprite AttackSprite => _sprites[Random.Range(0, _sprites.Length)];
        public AudioClip HitSound => _hitSounds[Random.Range(0, _hitSounds.Length)];

        public RuntimeAnimatorController HitAnimation { get; }
        public RuntimeAnimatorController AttackAnimation => _attackAnimation;
    }
}