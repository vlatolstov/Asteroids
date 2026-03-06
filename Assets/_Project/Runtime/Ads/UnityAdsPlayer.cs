using System;
using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.Models;
using UnityEngine;
using UnityEngine.Advertisements;

namespace _Project.Runtime.Ads
{
    public class UnityAdsPlayer : IAdsPlayer, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private const string RemoveAdsProductId = "RemoveAds";
        private readonly IAdsSettings _adsSettings;
        private readonly PlayerModel _playerModel;

        private int _loadAttemptsCount;
        private int _showAttemptsCount;

        public UnityAdsPlayer(IAdsSettings adsSettings, PlayerModel playerModel)
        {
            _adsSettings = adsSettings;
            _playerModel = playerModel;
        }

        public event Action<AdCompletionStatus> InterstitialAdPlayed;
        public event Action<AdCompletionStatus> RewardedAdPlayed;

        public void PlayInterstitialAd()
        {
            if (_playerModel.HasNonConsumable(RemoveAdsProductId))
            {
                Debug.Log("[Ads] Interstitial ad skipped because Remove Ads is purchased.");
                InterstitialAdPlayed?.Invoke(AdCompletionStatus.Completed);
                return;
            }

            ClearCounters();
            Debug.Log("Loading interstitial ad: " + _adsSettings.InterstitialAdId);
            Advertisement.Load(_adsSettings.InterstitialAdId, this);
        }

        public void PlayRewardedAd()
        {
            ClearCounters();
            Debug.Log("Loading rewarded ad: " + _adsSettings.RewardedAdId);
            Advertisement.Load(_adsSettings.RewardedAdId, this);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log("Unity Ad Loaded: " + placementId);
            Advertisement.Show(placementId, this);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Failed to load ad: {placementId}, error: {error}, message: {message}");
            if (_loadAttemptsCount++ <= _adsSettings.MaxLoadAttempts)
            {
                Debug.LogAssertion($"Retrying to load ad: {placementId}");
                Advertisement.Load(placementId, this);
            }
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Failed to play ad: {placementId}, error: {error}, message: {message}");
            if (_showAttemptsCount++ <= _adsSettings.MaxLoadAttempts)
            {
                Debug.LogAssertion($"Loading another {placementId} ad.");
                Advertisement.Load(placementId, this);
            }
        }

        public void OnUnityAdsShowStart(string placementId)
        { }

        public void OnUnityAdsShowClick(string placementId)
        { }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            var completionStatus = showCompletionState switch
            {
                UnityAdsShowCompletionState.COMPLETED => AdCompletionStatus.Completed,
                UnityAdsShowCompletionState.SKIPPED => AdCompletionStatus.Skipped,
                _ => AdCompletionStatus.Failed
            };
            if (placementId.Equals(_adsSettings.InterstitialAdId))
            {
                InterstitialAdPlayed?.Invoke(completionStatus);
            }
            else if (placementId.Equals(_adsSettings.RewardedAdId))
            {
                RewardedAdPlayed?.Invoke(completionStatus);
            }
            else
            {
                Debug.LogAssertion($"Unknown {placementId} ad completed. Handling not implemented.");
            }
        }

        private void ClearCounters()
        {
            _loadAttemptsCount = 0;
            _showAttemptsCount = 0;
        }
    }
}
