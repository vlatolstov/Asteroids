using UnityEngine;

namespace Runtime.Abstract.Visualization
{
    public interface IAttackRepresentation
    {
        Sprite AttackSprite { get; }
        RuntimeAnimatorController AttackAnimation { get; }
        AudioClip HitSound { get; }
        RuntimeAnimatorController HitAnimation { get; }
    }
}