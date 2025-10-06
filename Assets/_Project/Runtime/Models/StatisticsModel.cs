using _Project.Runtime.Data;

namespace _Project.Runtime.Models
{
    public class StatisticsModel
    {
        public int UfoDestroyed { get; private set; }
        public int LargeAsteroidsDestroyed { get; private set; }
        public int SmallAsteroidsDestroyed { get; private set; }
        
        public int ShipProjectileShots { get; private set; }
        public int ShipProjectileHits { get; private set; }
        public int ShipAoeAttacks { get; private set; }
        public int ShipAoeHits { get; private set; }
        public int UfoProjectileShots { get; private set; }

        public void AccountProjectileShot(ProjectileShot shot)
        {
            if (shot.Source == Source.Ship)
            {
                ShipProjectileShots++;
            }
            else
            {
                UfoProjectileShots++;
            }
        }

        public void AccountProjectileHit(ProjectileHit hit)
        {
            if (hit.Source == Source.Ship)
            {
                ShipProjectileHits++;
            }
        }

        public void AccountAoeShot(AoeAttackReleased shot)
        {
            if (shot.Source == Source.Ship)
            {
                ShipAoeAttacks++;
            }
        }
        
        public void AccountAoeHit(AoeHit hit)
        {
            if (hit.Source == Source.Ship)
            {
                ShipAoeHits++;
            }
        }
        
        public void AccountAsteroidDestroyed(AsteroidDestroyed data)
        {
            if (data.Size == AsteroidSize.Large)
            {
                LargeAsteroidsDestroyed++;
            }
            else
            {
                SmallAsteroidsDestroyed++;
            }
        }

        public void AccountUfoDestroyed(UfoDestroyed _)
        {
            UfoDestroyed++;
        }

        public void RefreshStatistics()
        {
            UfoDestroyed = 0;
            LargeAsteroidsDestroyed = 0;
            SmallAsteroidsDestroyed = 0;
            ShipProjectileShots = 0;
            ShipProjectileHits = 0;
            ShipAoeAttacks = 0;
            ShipAoeHits = 0;
            UfoProjectileShots = 0;
        }
    }
}