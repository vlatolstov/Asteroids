using System;

namespace _Project.Runtime.Abstract.Ads
{
    public interface IAdsPlayer
    {
        event Action<AdCompletionStatus> InterstitialAdPlayed;
        event Action<AdCompletionStatus> RewardedAdPlayed;
        void PlayInterstitialAd();
        void PlayRewardedAd();
    }

    public enum AdCompletionStatus
    {
        Completed,
        Skipped,
        Failed
    }
}