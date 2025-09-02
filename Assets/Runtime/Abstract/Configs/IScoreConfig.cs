namespace Runtime.Abstract.Configs
{
    public interface IScoreConfig
    {
        int BigAsteroidScore { get; }
        int SmallAsteroidScore { get; }
        int UfoScore { get; }
    }
}