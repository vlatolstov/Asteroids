using Runtime.Abstract.MVP;
using Runtime.Views;
using Runtime.Data;
using Runtime.Models;
using Zenject;

namespace Runtime.Contexts.Asteroids
{
    public class AsteroidsPresenter : BasePresenter<AsteroidsModel>
    {
        private readonly AsteroidView.Pool _pool;

        public AsteroidsPresenter(AsteroidsModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            AsteroidView.Pool pool) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            ForwardOn<AsteroidViewOffscreen>(publish: true);
            ForwardOn<AsteroidViewDestroyed>(publish: true);

            Model.Subscribe<AsteroidSpawnRequest>(OnSpawnRequest);
            Model.Subscribe<AsteroidDespawnRequest>(OnDespawnRequest);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            Model.Unsubscribe<AsteroidSpawnRequest>(OnSpawnRequest);
            Model.Unsubscribe<AsteroidDespawnRequest>(OnDespawnRequest);
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