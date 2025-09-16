namespace _Project.Runtime.Abstract.Configs
{
    public interface IScoreConfig
    {
        int LargeAsteroidScore { get; }
        int SmallAsteroidScore { get; }
        int UfoScore { get; }
    }
}