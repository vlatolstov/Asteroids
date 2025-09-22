using System;
using _Project.Runtime.Data;

namespace _Project.Runtime.Models
{
    public class GameModel
    {
        public event Action<GameState> GameStateChanged;
        
        public GameState CurrentState { get; private set; }

        public void SetGameState(GameState gameState)
        {
            CurrentState = gameState;
            GameStateChanged?.Invoke(gameState);
        }
    }
}