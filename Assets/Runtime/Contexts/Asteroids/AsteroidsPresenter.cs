using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Views;
using Runtime.Data;
using Zenject;

namespace Runtime.Contexts.Asteroids
{
    public class AsteroidPresenter : BasePresenter<IModel>
    {
        private readonly AsteroidView.Pool _pool;

        public AsteroidPresenter(IModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            AsteroidView.Pool pool) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            ForwardOn<AsteroidSpawnRequest>(publish: true);
            ForwardOn<AsteroidDespawnRequest>(publish: true);

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