using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using UnityEngine;

namespace Runtime.Data
{
    public readonly struct ProjectileHit : IEventData
    {
        public readonly GameObject Target;
        public readonly Vector2 Point;

        public ProjectileHit(GameObject target, Vector2 point)
        {
            Target = target;
            Point = point;
        }
    }

    public readonly struct ProjectileShoot : IEventData
    {
        public readonly IProjectileConfig Projectile;
        public readonly Vector2 Position;
        public readonly Vector2 Direction;
        public readonly Vector2 InheritVelocity;
        public readonly int Layer;

        public ProjectileShoot(IProjectileConfig projectile, Vector2 position, Vector2 direction,
            Vector2 inheritVelocity, int layer)
        {
            Projectile = projectile;
            Position = position;
            Direction = direction;
            InheritVelocity = inheritVelocity;
            Layer = layer;
        }
    }

    public readonly struct LaserShoot : IEventData
    { }
}