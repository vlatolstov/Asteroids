using Firebase.Analytics;

namespace _Project.Runtime.Analytics.Firebase
{
    public class FirebaseAnalyticsLogger : IAnalyticsLogger
    {
        public void LogGameStarted()
        {
            FirebaseAnalytics.LogEvent("game_started");
        }
        public void LogPlayerAoeWeaponShot()
        {
            FirebaseAnalytics.LogEvent("aoe_shoot");
        }

        public void LogGameEnded(EndgameStatistics stats)
        {
            var parameters = new[]
            {
                new Parameter("projectile_shots", stats.ProjectileShots),
                new Parameter("projectile_accuracy", stats.ProjectileAccuracy),
                new Parameter("aoe_shots", stats.AoeShots),
                new Parameter("asteroids_destroyed", stats.AsteroidsDestroyed),
                new Parameter("ufo_destroyed", stats.UfoDestroyed),
            };
            
            FirebaseAnalytics.LogEvent("game_ended_with_stats", parameters);
        }
    }
}