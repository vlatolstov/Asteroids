using System;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Score;
using _Project.Runtime.Settings;
using _Project.Runtime.Ship;
using _Project.Runtime.Views;
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

        private readonly HudView _hud;

        public HudPresenter(GameModel gameModel,
            ShipModel shipModel,
            ScoreModel scoreModel,
            CombatModel combatModel,
            StatisticsModel statisticsModel,
            ViewsContainer viewsContainer,
            GeneralVisualsConfig visuals)
        {
            _gameModel = gameModel;
            _shipModel = shipModel;
            _scoreModel = scoreModel;
            _combatModel = combatModel;
            _statisticsModel = statisticsModel;
            _visuals = visuals;

            _hud = viewsContainer.GetView<HudView>();

            _hud.PlayerSpawnButtonPressed += OnSpawnPlayerPressed;

            _shipModel.ShipPoseChanged += OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged += OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged += OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged += OnGameStateChanged;

            _scoreModel.TotalScoreChanged += OnScoreChanged;
            _scoreModel.BestScoreChanged += OnBestScoreChanged;
        }

        public void Initialize()
        {
            _hud.SetProjectileWeaponIcon(_visuals.ShipProjectileWeaponIcon);
            _hud.SetAoeWeaponIcon(_visuals.ShipAoeWeaponIcon);
            _hud.UpdateBestScore(_scoreModel.BestScore);
        }

        public void Dispose()
        {
            _hud.PlayerSpawnButtonPressed -= OnSpawnPlayerPressed;

            _shipModel.ShipPoseChanged -= OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged -= OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged -= OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged -= OnGameStateChanged;

            _scoreModel.TotalScoreChanged -= OnScoreChanged;
            _scoreModel.BestScoreChanged -= OnBestScoreChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                UpdateGameStatistics();
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
            _hud.UpdateBestScore(bestScore);
        }

        private void OnSpawnPlayerPressed()
        {
            _shipModel.RequestSpawn();
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
