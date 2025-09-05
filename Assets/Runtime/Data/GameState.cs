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
    
    public struct GameStateData : IStateData
    {
        public GameState State;

        public GameStateData(GameState state)
        {
            State = state;
        }
    }
}