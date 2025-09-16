using System;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using Zenject;

namespace _Project.Runtime.Models
{
    public class GameModel : BaseModel, IInitializable
    {
        public void Initialize()
        {
            Subscribe<ScoreAdded>(OnScoreAdded);
            Subscribe<GameStateData>(OnGameStateChanged);

            ChangeData(new GameStateData(GameState.Preparing));
        }

        private void OnGameStateChanged()
        {
            if (!TryGet(out GameStateData data))
            {
                return;
            }

            switch (data.State)
            {
                case GameState.Preparing:
                    break;
                case GameState.Gameplay:
                    OnGameplay();
                    break;
                case GameState.GameOver:
                    break;
                case GameState.Pause:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGameplay()
        {
            ChangeData(new TotalScore(0));
        }
        
        private void OnScoreAdded()
        {
            if (TryGet(out ScoreAdded added))
            {
                ChangeData<TotalScore>(prev => new TotalScore(prev.Amount + added.Amount));
            }
        }
    }
}