namespace _Project.Runtime.Abstract.Ads
{
    public interface IAdsSettings
    {
        string GameId { get; }
        
        string InterstitialAdId { get; }
        
        string RewardedAdId { get; }
        
        int MaxLoadAttempts { get; }
    }
}