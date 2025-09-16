using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Weapons;
using UnityEngine;

namespace _Project.Runtime.Data
{
    public readonly struct ProjectileHit : IEventData
    {
        public readonly ProjectileConfig Projectile;
        public readonly Vector2 Position;

        public ProjectileHit(ProjectileConfig projectile, Vector2 position)
        {
            Projectile = projectile;
            Position = position;
        }
    }

    public readonly struct ProjectileShoot : IEventData
    {
        public readonly Vector2 Position;
        public readonly Vector2 Direction;
        public readonly Vector2 InheritVelocity;
        public readonly int Layer;
        public readonly ProjectileWeaponConfig Weapon;

        public ProjectileShoot(Vector2 position, Vector2 direction,
            Vector2 inheritVelocity, int layer, ProjectileWeaponConfig weapon)
        {
            Position = position;
            Direction = direction;
            InheritVelocity = inheritVelocity;
            Layer = layer;
            Weapon = weapon;
        }
    }

   
    
    public readonly struct AoeAttackReleased : IEventData
    {
        public readonly Transform Emitter;
        public readonly AoeWeaponConfig Weapon;

        public AoeAttackReleased(Transform emitter, AoeWeaponConfig weapon)
        {
            Emitter = emitter;
            Weapon = weapon;
        }
    }

    public readonly struct AoeHit : IEventData
    {
        public readonly AoeAttackConfig Attack;
        public readonly Vector2 Position;

        public AoeHit(AoeAttackConfig attack, Vector2 position)
        {
            Attack = attack;
            Position = position;
        }
    }
}