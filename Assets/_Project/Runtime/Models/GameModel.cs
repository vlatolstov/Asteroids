using System;
using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.Data;
using Cysharp.Threading.Tasks;

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

        public void HandleLoadingFinished()
        {
            if (CurrentState == GameState.Gameplay)
            {
                return;
            }

            SetGameState(GameState.Preparing);
        }

        public void HandleShipSpawned()
        {
            UniTask.Void(async () =>
            {
                await UniTask.NextFrame();
                SetGameState(GameState.Gameplay);
            });
        }

        public void HandleShipDestroyed()
        {
            SetGameState(GameState.GameOver);
        }

        public bool TrySetPreparingAfterRewardedAd(AdCompletionStatus status)
        {
            if (status != AdCompletionStatus.Completed)
            {
                return false;
            }

            SetGameState(GameState.Preparing);
            return true;
        }
    }
}