using UnityEngine;

namespace Runtime.Abstract.Weapons
{
    public interface IProjectileConfig
    {
        Sprite Sprite { get; }
        float Size { get; }
        float Speed { get; }
        float Lifetime { get; }
    }
}