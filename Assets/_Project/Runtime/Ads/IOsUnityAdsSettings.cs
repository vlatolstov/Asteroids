using _Project.Runtime.Abstract.Ads;
using UnityEngine;

namespace _Project.Runtime.Ads
{
    [CreateAssetMenu(fileName = "IOsUnityAdsSettings", menuName = "Ads/Settings/iOS Settings")]
    public class IOsUnityAdsSettings : ScriptableObject, IAdsSettings
    {
        [field: SerializeField]
        public string GameId { get; private set; }

        [field: SerializeField]
        public string InterstitialAdId { get; private set; }

        [field: SerializeField]
        public string RewardedAdId { get; private set; }

        [field: SerializeField]
        public int MaxLoadAttempts { get; private set; }
    }
}