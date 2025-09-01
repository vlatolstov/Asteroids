namespace Runtime.Abstract.Configs
{
    public interface IScoreConfig
    {
        public int BigAsteroidScore { get; }
        public int SmallAsteroidScore { get; }
        public int UfoScore { get; }
    }
}