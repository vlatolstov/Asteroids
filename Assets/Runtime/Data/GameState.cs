using Runtime.Abstract.MVP;

namespace Runtime.Data
{
    public enum GameState
    {
        Preparing,
        Gameplay,
        GameOver,
        Pause
    }
    
    public readonly struct GameStateData : IStateData
    {
        public readonly GameState State;

        public GameStateData(GameState state)
        {
            State = state;
        }
    }
}