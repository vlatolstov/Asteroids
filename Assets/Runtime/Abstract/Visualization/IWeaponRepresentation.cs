using UnityEngine;

namespace Runtime.Abstract.Visualization
{
    public interface IWeaponRepresentation
    {
        // Sprite WeaponSprite { get; }
        AudioClip AttackSound { get; }
    }
}