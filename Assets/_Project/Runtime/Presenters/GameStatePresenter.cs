using System;
using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using _Project.Runtime.LoadingServices;
using _Project.Runtime.Models;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Ship;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class GameStatePresenter : IInitializable, IDisposable
    {
        private readonly GameModel _gameModel;
        private readonly ShipModel _shipModel;
        private readonly GameLoadingTasksProcessor _gameLoadingTasksProcessor;
        private readonly IAdsPlayer _adsPlayer;
        private readonly SceneLoader _sceneLoader;

        public GameStatePresenter(GameModel model, ShipModel shipModel, GameLoadingTasksProcessor gameLoadingTasksProcessor,
            IAdsPlayer adsPlayer, SceneLoader sceneLoader)
        {
            _gameModel = model;
            _shipModel = shipModel;
            _gameLoadingTasksProcessor = gameLoadingTasksProcessor;
            _adsPlayer = adsPlayer;
            _sceneLoader = sceneLoader;
        }
        
        public void Initialize()
        {
            _shipModel.ShipSpawned += OnShipSpawned;
            _shipModel.ShipDestroyed += OnShipDestroyed;

            _gameLoadingTasksProcessor.OnTasksFinished += OnLoadingFinished;
            _adsPlayer.RewardedAdPlayed += OnRewardedAdPlayed;
            _adsPlayer.InterstitialAdPlayed += OnInterstitialAdPlayed;
        }

        public void Dispose()
        {
            _shipModel.ShipSpawned -= OnShipSpawned;
            _shipModel.ShipDestroyed -= OnShipDestroyed;
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingFinished;
            _adsPlayer.RewardedAdPlayed -= OnRewardedAdPlayed;
            _adsPlayer.InterstitialAdPlayed -= OnInterstitialAdPlayed;
        }

        private void OnLoadingFinished()
        {
            _gameLoadingTasksProcessor.OnTasksFinished -= OnLoadingFinished;
            _gameModel.HandleLoadingFinished();
        }

        private void OnShipSpawned(ShipSpawned _)
        {
            _gameModel.HandleShipSpawned();
        }

        private void OnShipDestroyed(ShipDestroyed _)
        {
            _gameModel.HandleShipDestroyed();
        }

        private void OnRewardedAdPlayed(AdCompletionStatus status)
        {
            _gameModel.TrySetPreparingAfterRewardedAd(status);
        }

        private void OnInterstitialAdPlayed(AdCompletionStatus _)
        {
            UniTask.Void(async () => { await _sceneLoader.LoadSceneAsync(Scenes.Menu); });
        }
    }
}
