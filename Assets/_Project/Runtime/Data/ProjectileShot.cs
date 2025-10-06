using _Project.Runtime.Weapons;
using UnityEngine;

namespace _Project.Runtime.Data
{
    public readonly struct ProjectileHit
    {
        public readonly ProjectileConfig Projectile;
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;

        public readonly Source Source;

        public ProjectileHit(ProjectileConfig projectile, Vector2 position, Quaternion rotation, Vector2 scale,
            Source source)
        {
            Projectile = projectile;
            Position = position;
            Source = source;
            Rotation = rotation;
            Scale = scale;
        }
    }

    public struct ProjectileShot
    {
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;

        public readonly Vector2 Direction;
        public readonly Vector2 InheritVelocity;
        public readonly int Layer;
        public readonly ProjectileWeaponConfig Weapon;
        public readonly Source Source;

        public ProjectileShot(Vector2 position, Quaternion rotation,
            Vector2 scale, Vector2 direction,
            Vector2 inheritVelocity, int layer, ProjectileWeaponConfig weapon, Source source)
        {
            Position = position;
            Direction = direction;
            InheritVelocity = inheritVelocity;
            Layer = layer;
            Weapon = weapon;
            Source = source;
            Rotation = rotation;
            Scale = scale;
        }
    }

    public readonly struct AoeAttackReleased
    {
        public readonly Transform Emitter;
        public readonly AoeWeaponConfig Weapon;
        public readonly Source Source;

        public AoeAttackReleased(Transform emitter, AoeWeaponConfig weapon, Source source)
        {
            Emitter = emitter;
            Weapon = weapon;
            Source = source;
        }
    }

    public readonly struct AoeHit
    {
        public readonly AoeAttackConfig Attack;
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;
        public readonly Source Source;

        public AoeHit(AoeAttackConfig attack, Vector2 position, Quaternion rotation, Vector2 scale, Source source)
        {
            Attack = attack;
            Position = position;
            Source = source;
            Rotation = rotation;
            Scale = scale;
        }
    }


    public enum Source
    {
        Undefined = 0,
        Ship,
        Ufo,
    }
}