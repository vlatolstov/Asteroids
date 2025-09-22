using System;

namespace _Project.Runtime.Data
{
    public struct ProjectileWeaponState : IEquatable<ProjectileWeaponState>
    {
        public readonly float Cooldown;
        public readonly float ReloadRatio;

        public ProjectileWeaponState(float cooldown, float reloadRatio)
        {
            Cooldown = cooldown;
            ReloadRatio = reloadRatio;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(ProjectileWeaponState other)
        {
            return Cooldown.Equals(other.Cooldown);
        }

        public override int GetHashCode()
        {
            return Cooldown.GetHashCode();
        }

        public static bool operator ==(ProjectileWeaponState left, ProjectileWeaponState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProjectileWeaponState left, ProjectileWeaponState right)
        {
            return !left.Equals(right);
        }
    }

    public struct AoeWeaponState : IEquatable<AoeWeaponState>
    {
        public readonly int MaxCharges;
        public readonly int Charges;
        public readonly float Cooldown;
        public readonly float RechargeRatio;

        public AoeWeaponState(int maxCharges, int charges, float cooldown, float rechargeRatio)
        {
            MaxCharges = maxCharges;
            Charges = charges;
            Cooldown = cooldown;
            RechargeRatio = rechargeRatio;
        }

        public bool Equals(AoeWeaponState other)
        {
            return MaxCharges == other.MaxCharges && Charges == other.Charges && Cooldown.Equals(other.Cooldown) && RechargeRatio.Equals(other.RechargeRatio);
        }

        public override bool Equals(object obj)
        {
            return obj is AoeWeaponState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MaxCharges, Charges, Cooldown, RechargeRatio);
        }

        public static bool operator ==(AoeWeaponState left, AoeWeaponState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AoeWeaponState left, AoeWeaponState right)
        {
            return !left.Equals(right);
        }
    }
}