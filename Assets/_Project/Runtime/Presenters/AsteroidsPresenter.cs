using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Views;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class AsteroidsPresenter : BasePresenter<AsteroidsModel>
    {
        private readonly AsteroidView.Pool _pool;
        private readonly GameModel _gameModel;
        
        public AsteroidsPresenter(AsteroidsModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            AsteroidView.Pool pool, GameModel gameModel) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
            _gameModel = gameModel;
        }

        public override void Initialize()
        {
            ForwardOn<AsteroidViewOffscreen>(publish: true);
            ForwardOn<AsteroidDestroyed>(publish: true);

            AddUnsub(Model.Subscribe<AsteroidSpawnCommand>(OnSpawnCommand));
            AddUnsub(Model.Subscribe<AsteroidDespawnCommand>(OnDespawnCommand));
            
            AddUnsub(_gameModel.Subscribe<GameStateData>(OnGameStateChanged));
        }

        private void OnSpawnCommand()
        {
            if (!Model.TryGet(out AsteroidSpawnCommand request))
            {
                return;
            }

            _pool.Spawn(request);
        }

        private void OnDespawnCommand()
        {
            if (!Model.TryGet(out AsteroidDespawnCommand request))
            {
                return;
            }

            var view = ViewsContainer.GetViewById(request.ViewId);

            if (!view || view is not AsteroidView asteroidView)
            {
                return;
            }

            _pool.Despawn(asteroidView);
        }

        private void OnGameStateChanged()
        {
            if (_gameModel.TryGet(out GameStateData stateData))
            {
                Model.SetGameState(stateData.State);
            }
        }
    }
}