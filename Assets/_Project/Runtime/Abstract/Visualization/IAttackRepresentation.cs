using UnityEngine;

namespace _Project.Runtime.Abstract.Visualization
{
    public interface IAttackRepresentation
    {
        Sprite AttackSprite { get; }
        RuntimeAnimatorController AttackAnimation { get; }
        AudioClip HitSound { get; }
        RuntimeAnimatorController HitAnimation { get; }
    }
}