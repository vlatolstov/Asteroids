using System;
using _Project.Runtime.Data;

namespace _Project.Runtime.Models
{
    public class CombatModel
    {
        public event Action<ProjectileShot> ProjectileShot;
        public event Action<ProjectileHit> ProjectileHit;
        public event Action<AoeAttackReleased> AoeAttackReleased;
        public event Action<AoeHit> AoeHit;
        
        public void HandleProjectileShot(ProjectileShot shot)
        {
            ProjectileShot?.Invoke(shot);
        }

        public void HandleProjectileHit(ProjectileHit hit)
        {
            ProjectileHit?.Invoke(hit);
        }

        public void HandleAoeAttackReleased(AoeAttackReleased attack)
        {
            AoeAttackReleased?.Invoke(attack);
        }

        public void HandleAoeHit(AoeHit hit)
        {
            AoeHit?.Invoke(hit);
        }
    }
}