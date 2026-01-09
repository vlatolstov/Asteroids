using _Project.Runtime.RemoteConfig;
using _Project.Runtime.Weapons;
using UnityEngine;

namespace _Project.Runtime.Data
{
    public readonly struct ProjectileHit
    {
        public readonly ProjectileConfig Projectile;
        public readonly ProjectileAttackData AttackData;
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;

        public readonly Source Source;

        public ProjectileHit(ProjectileConfig projectile, ProjectileAttackData attackData, Vector2 position,
            Quaternion rotation, Vector2 scale, Source source)
        {
            Projectile = projectile;
            AttackData = attackData;
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
        public readonly ProjectileAttackData AttackData;
        public readonly Source Source;

        public ProjectileShot(Vector2 position, Quaternion rotation,
            Vector2 scale, Vector2 direction,
            Vector2 inheritVelocity, int layer, ProjectileWeaponConfig weapon, ProjectileAttackData attackData,
            Source source)
        {
            Position = position;
            Direction = direction;
            InheritVelocity = inheritVelocity;
            Layer = layer;
            Weapon = weapon;
            AttackData = attackData;
            Source = source;
            Rotation = rotation;
            Scale = scale;
        }
    }

    public readonly struct AoeAttackReleased
    {
        public readonly Transform Emitter;
        public readonly AoeWeaponConfig Weapon;
        public readonly AoeWeaponData WeaponData;
        public readonly AoeAttackData AttackData;
        public readonly Source Source;

        public AoeAttackReleased(Transform emitter, AoeWeaponConfig weapon, AoeWeaponData weaponData,
            AoeAttackData attackData, Source source)
        {
            Emitter = emitter;
            Weapon = weapon;
            WeaponData = weaponData;
            AttackData = attackData;
            Source = source;
        }
    }

    public readonly struct AoeHit
    {
        public readonly AoeAttackConfig Attack;
        public readonly AoeAttackData AttackData;
        public readonly Vector2 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2 Scale;
        public readonly Source Source;

        public AoeHit(AoeAttackConfig attack, AoeAttackData attackData, Vector2 position, Quaternion rotation,
            Vector2 scale, Source source)
        {
            Attack = attack;
            AttackData = attackData;
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
