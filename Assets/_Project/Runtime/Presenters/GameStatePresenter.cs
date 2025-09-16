using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class GameStatePresenter : BasePresenter<GameModel>
    {
        public GameStatePresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus) : base(model,
            viewsContainer, signalBus)
        {
        }

        public override void Initialize()
        {
            MutateOn<GameStateData, ShipSpawned>(OnShipSpawned);
            MutateOn<GameStateData, ShipDestroyed>(OnShipDestroyed);
        }

        private GameStateData OnShipSpawned(GameStateData previousState, ShipSpawned signal)
        {
            return new GameStateData(GameState.Gameplay);
        }
        
        private GameStateData OnShipDestroyed(GameStateData previousState, ShipDestroyed signal)
        {
            return new GameStateData(GameState.GameOver);
        }
    }
}