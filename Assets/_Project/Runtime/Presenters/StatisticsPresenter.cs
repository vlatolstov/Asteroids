using _Project.Runtime.Asteroid;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Ufo;

namespace _Project.Runtime.Presenters
{
    public class StatisticsPresenter
    {
        private StatisticsModel _statisticsModel;
        private GameModel _gameModel;
        private CombatModel _combatModel;
        private UfoModel _ufoModel;
        private AsteroidsModel _asteroidsModel;

        public StatisticsPresenter(
            StatisticsModel statisticsModel, GameModel gameModel, CombatModel combatModel, UfoModel ufoModel,
            AsteroidsModel asteroidsModel)
        {
            _statisticsModel = statisticsModel;
            _gameModel = gameModel;
            _combatModel = combatModel;
            _ufoModel = ufoModel;
            _asteroidsModel = asteroidsModel;

            _gameModel.GameStateChanged += OnGameStateChanged;
            
            _combatModel.ProjectileShot += OnProjectileShot;
            _combatModel.ProjectileHit += OnProjectileHit;
            _combatModel.AoeAttackReleased += OnAoeAttackReleased;
            _combatModel.AoeHit += OnAoeHit;
            
            _ufoModel.UfoDestroyed += OnUfoDestroyed;
            _asteroidsModel.AsteroidDestroyed += OnAsteroidDestroyed;
        }

        private void OnAsteroidDestroyed(AsteroidDestroyed obj)
        {
            _statisticsModel.AccountAsteroidDestroyed(obj);
        }

        private void OnUfoDestroyed(UfoDestroyed obj)
        {
            _statisticsModel.AccountUfoDestroyed(obj);
        }

        private void OnAoeHit(AoeHit obj)
        {
            _statisticsModel.AccountAoeHit(obj);
        }

        private void OnAoeAttackReleased(AoeAttackReleased obj)
        {
            _statisticsModel.AccountAoeShot(obj);
        }

        private void OnProjectileHit(ProjectileHit obj)
        {
            _statisticsModel.AccountProjectileHit(obj);
        }

        private void OnProjectileShot(ProjectileShot obj)
        {
            _statisticsModel.AccountProjectileShot(obj);
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state is GameState.Preparing or GameState.Gameplay)
            {
                _statisticsModel.RefreshStatistics();
            }
        }
    }
}