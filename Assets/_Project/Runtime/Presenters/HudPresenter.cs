using System;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Score;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class HudPresenter : IInitializable, IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly ScoreModel _scoreModel;
        private readonly CombatModel _combatModel;
        private readonly StatisticsModel _statisticsModel;
        private readonly ShipModel _shipModel;
        private readonly GeneralVisualsConfig _visuals;
        private readonly SceneLoader _sceneLoader;

        private readonly HudView _hud;
        private int _lastBestScore;

        public HudPresenter(GameModel gameModel,
            ShipModel shipModel,
            ScoreModel scoreModel,
            CombatModel combatModel,
            StatisticsModel statisticsModel,
            ViewsContainer viewsContainer,
            GeneralVisualsConfig visuals,
            SceneLoader sceneLoader)
        {
            _gameModel = gameModel;
            _shipModel = shipModel;
            _scoreModel = scoreModel;
            _combatModel = combatModel;
            _statisticsModel = statisticsModel;
            _visuals = visuals;
            _sceneLoader = sceneLoader;
            
            _hud = viewsContainer.GetView<HudView>();
        }

        public void Initialize()
        {
            _lastBestScore = _scoreModel.BestScore;

            if (_hud != null)
            {
                _hud.RespawnButtonPressed += OnRespawnButtonPressed;
                _hud.BackToMenuButtonPressed += OnBackToMenuButtonPressed;
                _hud.SetNewRecordAchieved(false);
            }

            _shipModel.ShipPoseChanged += OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged += OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged += OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged += OnGameStateChanged;

            _scoreModel.TotalScoreChanged += OnScoreChanged;
            _scoreModel.BestScoreChanged += OnBestScoreChanged;
            
            _hud.SetProjectileWeaponIcon(_visuals.ShipProjectileWeaponIcon);
            _hud.SetAoeWeaponIcon(_visuals.ShipAoeWeaponIcon);
            _hud.UpdateBestScore(_scoreModel.BestScore);

            _shipModel.RequestSpawn();
        }

        public void Dispose()
        {
            if (_hud != null)
            {
                _hud.RespawnButtonPressed -= OnRespawnButtonPressed;
                _hud.BackToMenuButtonPressed -= OnBackToMenuButtonPressed;
            }

            _shipModel.ShipPoseChanged -= OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged -= OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged -= OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged -= OnGameStateChanged;

            _scoreModel.TotalScoreChanged -= OnScoreChanged;
            _scoreModel.BestScoreChanged -= OnBestScoreChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.GameOver:
                    UpdateGameStatistics();
                    break;
                case GameState.Preparing:
                case GameState.Gameplay:
                    _hud?.SetNewRecordAchieved(false);
                    break;
            }

            _hud.UpdateGameState(state);
        }

        private void OnPoseChanged(ShipPose pose)
        {
            _hud.UpdatePoseData(pose.Position, pose.Velocity, pose.AngleRadians);
        }

        private void OnProjectileWeaponStateChanged(ProjectileWeaponState state)
        {
            _hud.UpdateProjectileWeaponData(state.Cooldown, state.ReloadRatio);
        }

        private void OnAoeWeaponStateChanged(AoeWeaponState state)
        {
            _hud.UpdateAoeWeaponData(state.MaxCharges, state.Charges, state.RechargeRatio, state.Cooldown,
                state.ReloadRatio);
        }

        private void OnScoreChanged(int totalScore)
        {
            _hud.UpdateScore(totalScore);
        }

        private void OnBestScoreChanged(int bestScore)
        {
            bool isNewRecord = bestScore > _lastBestScore;
            _lastBestScore = bestScore;

            _hud.UpdateBestScore(bestScore);
            if (isNewRecord)
            {
                _hud.SetNewRecordAchieved(true);
            }
        }

        private void OnRespawnButtonPressed()
        {
            _gameModel.SetGameState(GameState.Preparing);
            _shipModel.RequestSpawn();
            _hud?.SetNewRecordAchieved(false);
        }

        private void OnBackToMenuButtonPressed()
        {
            UniTask.Void(async () =>
            {
                await _sceneLoader.LoadSceneAsync(Constants.Scenes.Menu);
            });
        }

        private void UpdateGameStatistics()
        {
            var stats = _statisticsModel;

            int asteroidTotal = stats.LargeAsteroidsDestroyed + stats.SmallAsteroidsDestroyed;
            float projectileAccuracy = stats.ShipProjectileShots > 0
                ? (float)stats.ShipProjectileHits / stats.ShipProjectileShots
                : 0f;

            int projectilePercent = Mathf.RoundToInt(projectileAccuracy * 100f);

            string projectileLine = stats.ShipProjectileShots > 0
                ? $"Guns: {stats.ShipProjectileHits}/{stats.ShipProjectileShots} hits ({projectilePercent}%)"
                : "Guns: no shots";

            string aoeLine = stats.ShipAoeAttacks > 0
                ? $"Laser: {stats.ShipAoeHits} hits from {stats.ShipAoeAttacks}"
                : "Laser: no bursts";

            string summary =
                $"Asteroids: {asteroidTotal} ({stats.LargeAsteroidsDestroyed} large /{stats.SmallAsteroidsDestroyed} small)\n" +
                $"UFOs: {stats.UfoDestroyed}\n" +
                projectileLine + "\n" +
                aoeLine;

            _hud.SetStatisticsSummary(summary);
        }
    }
}
