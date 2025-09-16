using UnityEngine;

namespace _Project.Runtime.Abstract.Weapons
{
    public interface IProjectileConfig
    {
        Vector2 Size { get; }
        float Speed { get; }
        float Lifetime { get; }
    }
}