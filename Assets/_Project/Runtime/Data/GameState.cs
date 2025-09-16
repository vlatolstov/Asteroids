using _Project.Runtime.Abstract.MVP;

namespace _Project.Runtime.Data
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