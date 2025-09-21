using System;
using _Project.Runtime.Abstract.Visualization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Runtime.Weapons
{
    [CreateAssetMenu(fileName = "AoeAttackConfig", menuName = "Settings/Weapons/AOE Attack", order = 0)]
    public class AoeAttackConfig : ScriptableObject, IAttackRepresentation
    {
        [Serializable]
        public enum AttachMode
        {
            FollowEmitter,
            Static
        }

        [Header("Logic")]
        [field: SerializeField]
        public float Length { get; private set; }

        [field: SerializeField]
        public float Width { get; private set; }

        [field: SerializeField]
        public float Duration { get; private set; }

        [field: SerializeField]
        public AttachMode Mode { get; private set; }

        [Header("Representation")]
        [SerializeField]
        [Tooltip("Range of the projectile appearances, randomly picked to spawn")]
        private Sprite[] _sprites;

        public Sprite AttackSprite => _sprites[Random.Range(0, _sprites.Length)];

        [SerializeField]
        [Tooltip("Range of the projectile hit sounds, randomly picked to play at hit")]
        private AudioClip[] _hitSounds;

        public AudioClip HitSound => _hitSounds[Random.Range(0, _hitSounds.Length)];

        [field: SerializeField]
        public RuntimeAnimatorController AttackAnimation { get; private set; }

        [field: SerializeField]
        public RuntimeAnimatorController HitAnimation { get; private set; }
    }
}