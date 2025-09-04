using Runtime.Abstract.MVP;

namespace Runtime.Data
{
    public readonly struct ScoreAdded : IEventData
    {
        public readonly int Amount;

        public ScoreAdded(int amount)
        {
            Amount = amount;
        }
    }

    public readonly struct TotalScore : IStateData
    {
        public readonly int Amount;

        public TotalScore(int amount)
        {
            Amount = amount;
        }
    }
}