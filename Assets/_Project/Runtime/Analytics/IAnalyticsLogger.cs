namespace _Project.Runtime.Analytics
{
    public interface IAnalyticsLogger
    {
        void LogGameStarted();
        void LogPlayerAoeWeaponShot();
        void LogGameEnded(EndgameStatistics stats);
    }
}