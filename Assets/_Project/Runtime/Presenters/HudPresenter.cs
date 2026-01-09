using System;
using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.Score;
using _Project.Runtime.Services;
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
        private readonly StatisticsModel _statisticsModel;
        private readonly ShipModel _shipModel;
        private readonly GameLoadingTasksProcessor _gameLoadingTasksProcessor;
        private readonly IResourcesService _resourcesService;
        private readonly IAdsPlayer _adsPlayer;
        private readonly SceneAssetProvider _assetProvider;

        private HudView _hud;
        private GeneralVisualsResource _visuals;
        private bool _hudReady;

        public HudPresenter(GameModel gameModel,
            ShipModel shipModel,
            ScoreModel scoreModel,
            StatisticsModel statisticsModel,
            GameLoadingTasksProcessor gameLoadingTasksProcessor,
            SceneAssetProvider assetProvider,
            IResourcesService resourcesService,
            IAdsPlayer adsPlayer)
        {
            _gameModel = gameModel;
            _shipModel = shipModel;
            _scoreModel = scoreModel;
            _statisticsModel = statisticsModel;
            _gameLoadingTasksProcessor = gameLoadingTasksProcessor;
            _assetProvider = assetProvider;
            _resourcesService = resourcesService;
            _adsPlayer = adsPlayer;
        }

        public void Initialize()
        {
            if (_gameLoadingTasksProcessor.IsFinished)
            {
                OnLoadingTaskFinished();
            }
            else
            {
                _gameLoadingTasksProcessor.OnTasksFinished += OnLoadingTaskFinished;
            }
        }


        public void Dispose()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;

            if (_hud)
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
            _scoreModel.NewRecordChanged -= OnNewRecordChanged;
        }

        private void OnLoadingTaskFinished()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingTaskFinished;
            InitHud();
        }

        private void InitHud()
        {
            if (_hudReady)
            {
                return;
            }

            if (!_assetProvider.TryGetLoadedComponent(out _hud) ||
                !_hud)
            {
                Debug.LogError("HudView not provided");
                return;
            }

            _visuals ??= _resourcesService.Get<GeneralVisualsResource>(AddressablesResourcePaths.General.GeneralVisuals);

            _hud.RespawnButtonPressed += OnRespawnButtonPressed;
            _hud.BackToMenuButtonPressed += OnBackToMenuButtonPressed;

            _shipModel.ShipPoseChanged += OnPoseChanged;
            _shipModel.ProjectileWeaponStateChanged += OnProjectileWeaponStateChanged;
            _shipModel.AoeWeaponStateChanged += OnAoeWeaponStateChanged;

            _gameModel.GameStateChanged += OnGameStateChanged;

            _scoreModel.TotalScoreChanged += OnScoreChanged;
            _scoreModel.BestScoreChanged += OnBestScoreChanged;
            _scoreModel.NewRecordChanged += OnNewRecordChanged;

            _hud.SetProjectileWeaponIcon(_visuals.ShipProjectileWeaponIcon);
            _hud.SetAoeWeaponIcon(_visuals.ShipAoeWeaponIcon);
            _hud.UpdateBestScore(_scoreModel.BestScore);
            _hud.SetNewRecordAchieved(_scoreModel.IsNewRecord);

            _hudReady = true;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                _hud?.SetStatisticsSummary(_statisticsModel.BuildSummary());
            }

            _hud?.UpdateGameState(state);
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

        private void OnNewRecordChanged(bool isNewRecord)
        {
            _hud?.SetNewRecordAchieved(isNewRecord);
        }

        private void OnRespawnButtonPressed()
        {
            _adsPlayer.PlayRewardedAd();
        }

        private void OnBackToMenuButtonPressed()
        {
            _adsPlayer.PlayInterstitialAd();
        }

    }
}
