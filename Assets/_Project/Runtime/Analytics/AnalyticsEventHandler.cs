using _Project.Runtime.Data;
using _Project.Runtime.Models;
using Firebase;

namespace _Project.Runtime.Analytics
{
    public class AnalyticsEventHandler
    {
        private readonly IAnalyticsLogger _analyticsLogger;

        private readonly StatisticsModel _statisticsModel;
        private readonly CombatModel _combatModel;
        private readonly GameModel _gameModel;

        public AnalyticsEventHandler(IAnalyticsLogger analyticsLogger, StatisticsModel statisticsModel,
            GameModel gameModel, CombatModel combatModel)
        {
            _analyticsLogger = analyticsLogger;
            _statisticsModel = statisticsModel;
            _gameModel = gameModel;
            _combatModel = combatModel;

            _gameModel.GameStateChanged += OnGameStateChanged;
            _combatModel.AoeAttackReleased += OnAoeAttackReleased;
        }

        private void OnAoeAttackReleased(AoeAttackReleased _)
        {
            _analyticsLogger.LogPlayerAoeWeaponShot();
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Gameplay:
                    _analyticsLogger.LogGameStarted();
                    break;
                case GameState.GameOver:
                    var stats = GetEndgameStatistics();
                    _analyticsLogger.LogGameEnded(stats);
                    break;
            }
        }

        private EndgameStatistics GetEndgameStatistics()
        {
            return new EndgameStatistics
            {
                ProjectileShots = _statisticsModel.ShipProjectileShots,
                ProjectileAccuracy = (float)_statisticsModel.ShipProjectileHits / _statisticsModel.ShipProjectileShots,
                AoeShots = _statisticsModel.ShipAoeAttacks,
                AsteroidsDestroyed =
                    _statisticsModel.LargeAsteroidsDestroyed + _statisticsModel.SmallAsteroidsDestroyed,
                UfoDestroyed = _statisticsModel.UfoDestroyed
            };
        }
    }
}