using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using Zenject;

namespace Runtime.Presenters
{
    public class UfoPresenter : BasePresenter<UfoModel>
    {
        private readonly ShipModel _shipModel;
        private readonly GameModel _gameModel;
        private readonly UfoView.Pool _pool;
        
        private ShipPose _targetShip;
        private GameState _gameState;

        public UfoPresenter(UfoModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipModel shipModel,
            GameModel gameModel, UfoView.Pool pool) : base(model,
            viewsContainer, signalBus)
        {
            _shipModel = shipModel;
            _gameModel = gameModel;
            _pool = pool;
        }

        public override void Initialize()
        {
            ForwardOn<UfoSpawned>(publish: true);
            ForwardOn<UfoDestroyed>(publish: true);

            AddUnsub(_shipModel.Subscribe<ShipPose>(OnShipPoseChanged));
            AddUnsub(_gameModel.Subscribe<GameStateData>(OnGameStateChanged));

            AddUnsub(Model.Subscribe<UfoSpawnCommand>(OnUfoSpawnCommand));
            AddUnsub(Model.Subscribe<UfoDespawnCommand>(OnUfoDespawnCommand));

            OnShipPoseChanged();
            OnGameStateChanged();
        }

        private void OnShipPoseChanged()
        {
            if (_shipModel.TryGet(out ShipPose shipPose))
            {
                foreach (var ufoView in ViewsContainer.GetViews<UfoView>())
                {
                    _targetShip = shipPose;
                    ufoView.UpdateShipPose(shipPose);
                }
            }
        }

        private void OnGameStateChanged()
        {
            if (_gameModel.TryGet(out GameStateData data))
            {
                foreach (var ufoView in ViewsContainer.GetViews<UfoView>())
                {
                    _gameState = data.State;
                    ufoView.UpdateGameState(data.State);
                }
            }
        }

        private void OnUfoSpawnCommand()
        {
            if (Model.TryGet(out UfoSpawnCommand spawn))
            {
                var ufo = _pool.Spawn(spawn);
                ufo.UpdateShipPose(_targetShip);
                ufo.UpdateGameState(GameState.Gameplay);
            }
        }

        private void OnUfoDespawnCommand()
        {
            if (Model.TryGet(out UfoDespawnCommand despawn))
            {
                var view = ViewsContainer.GetViewById(despawn.ViewId);

                if (view is UfoView ufo)
                {
                    _pool.Despawn(ufo);
                }
            }
        }
    }
}