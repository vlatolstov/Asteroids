using UnityEngine;

namespace Runtime.Abstract.Weapons
{
    public interface IProjectileConfig
    {
        Sprite Sprite { get; }
        Vector2 Size { get; }
        float Speed { get; }
        float Lifetime { get; }
    }
}