using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using Zenject;

namespace Runtime.Presenters
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

            AddUnsub(Model.Subscribe<AsteroidSpawnRequest>(OnSpawnRequest));
            AddUnsub(Model.Subscribe<AsteroidDespawnRequest>(OnDespawnRequest));
        }

        private void OnSpawnRequest()
        {
            if (!Model.TryGet(out AsteroidSpawnRequest request))
            {
                return;
            }

            _pool.Spawn(request);
        }

        private void OnDespawnRequest()
        {
            if (!Model.TryGet(out AsteroidDespawnRequest request))
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
    }
}