using _Project.Runtime.Abstract.Ads;
using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;

namespace _Project.Runtime.Ads
{
    public class UnityAdsInitializer : IInitializable, IAdsInitializer, IUnityAdsInitializationListener
    {
        private readonly string _gameId;
        private readonly bool _testMode;

        public UnityAdsInitializer(IAdsSettings settings)
        {
            _gameId = settings.GameId;
            _testMode = false;
#if UNITY_EDITOR
            _testMode = true;
#endif
        }

        public void Initialize()
        {
            InitializeAds();
        }

        public void InitializeAds()
        {
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _testMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialized");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Error occured during Unity Ads initialization: " +
                      $"case {error}" +
                      $"{message}");
        }
    }
}