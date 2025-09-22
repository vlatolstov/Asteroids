using System;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Views;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class HudPresenter : IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly ScoreModel _scoreModel;
        private readonly CombatModel _combatModel;
        private readonly ShipModel _shipModel;

        private readonly HudView _hud;

        public HudPresenter(GameModel gameModel, ShipModel shipModel, ScoreModel scoreModel, CombatModel combatModel,
            ViewsContainer viewsContainer)
        {
            _gameModel = gameModel;
            _shipModel = shipModel;
            _scoreModel = scoreModel;
            _combatModel = combatModel;

            _hud = viewsContainer.GetView<HudView>();

            _hud.PlayerSpawnButtonPressed += OnSpawnPlayerPressed;

            _shipModel.ShipPoseChanged += OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged += OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged += OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged += OnGameStateChanged;

            _scoreModel.TotalScoreChanged += OnScoreChanged;
        }


        public void Dispose()
        {
            _hud.PlayerSpawnButtonPressed -= OnSpawnPlayerPressed;

            _shipModel.ShipPoseChanged -= OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged -= OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged -= OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged -= OnGameStateChanged;

            _scoreModel.TotalScoreChanged -= OnScoreChanged;
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
            _hud.UpdateAoeWeaponData(state.MaxCharges, state.Charges, state.RechargeRatio);
        }

        private void OnScoreChanged(int totalScore)
        {
            _hud.UpdateScore(totalScore);
        }

        private void OnSpawnPlayerPressed()
        {
            _shipModel.RequestSpawn();
        }

        private void UpdateGameStatistics()
        {
            //TODO add combat statistics
            var m = _combatModel;
        }
    }
}