using System;
using _Project.Runtime.Data;

namespace _Project.Runtime.Models
{
    public class CombatModel
    {
        public int ShipProjectileShots { get; private set; }
        public int ShipProjectileHits { get; private set; }
        public int ShipAoeAttacks { get; private set; }
        public int ShipAoeHits { get; private set; }
        public int UfoProjectileShots { get; private set; }
        public int UfoProjectileHits { get; private set; }

        public event Action<ProjectileShot> ProjectileShot;
        public event Action<ProjectileHit> ProjectileHit;
        public event Action<AoeAttackReleased> AoeAttackReleased;
        public event Action<AoeHit> AoeHit;
        
        public void HandleProjectileShot(ProjectileShot shot)
        {
            ProcessCombatEvent(shot.Source, CombatEvent.Shot, WeaponType.Projectile);
            ProjectileShot?.Invoke(shot);
        }

        public void HandleProjectileHit(ProjectileHit hit)
        {
            ProcessCombatEvent(hit.Source, CombatEvent.Hit, WeaponType.Projectile);
            ProjectileHit?.Invoke(hit);
        }

        public void HandleAoeAttackReleased(AoeAttackReleased attack)
        {
            ProcessCombatEvent(attack.Source, CombatEvent.Shot, WeaponType.Aoe);
            AoeAttackReleased?.Invoke(attack);
        }

        public void HandleAoeHit(AoeHit hit)
        {
            ProcessCombatEvent(hit.Source, CombatEvent.Hit, WeaponType.Aoe);
            AoeHit?.Invoke(hit);
        }

        private void ProcessCombatEvent(Source source, CombatEvent ev, WeaponType weapon)
        {
            if (source == Source.Undefined ||
                ev == CombatEvent.Undefined ||
                weapon == WeaponType.Undefined)
            {
                throw new ArgumentException("Undefined combat event");
            }

            switch (source, ev, weapon)
            {
                case (Source.Ship, CombatEvent.Shot, WeaponType.Projectile):
                    ShipProjectileShots++;
                    break;
                case (Source.Ship, CombatEvent.Hit, WeaponType.Projectile):
                    ShipProjectileHits++;
                    break;
                case (Source.Ship, CombatEvent.Shot, WeaponType.Aoe):
                    ShipAoeAttacks++;
                    break;
                case (Source.Ship, CombatEvent.Hit, WeaponType.Aoe):
                    ShipAoeHits++;
                    break;
                case (Source.Ufo, CombatEvent.Shot, WeaponType.Projectile):
                    UfoProjectileShots++;
                    break;
                case (Source.Ufo, CombatEvent.Hit, WeaponType.Projectile):
                    UfoProjectileHits++;
                    break;
            }
        }

        public void RefreshStatistics()
        {
            ShipProjectileShots = 0;
            ShipProjectileHits = 0;
            ShipAoeAttacks = 0;
            ShipAoeHits = 0;
            UfoProjectileShots = 0;
            UfoProjectileHits = 0;
        }
    }
}