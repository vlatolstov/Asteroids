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
    //TODO зарефакторить
    public class AsteroidPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private readonly AsteroidView.Pool _pool;
        private readonly HashSet<BaseView> _attached;

        public AsteroidPresenter(
            IModel model, IViewsContainer views,
            AsteroidView.Pool pool)
            : base(model, views)
        {
            _pool = pool;
            _attached = new HashSet<BaseView>();
        }

        public void Initialize()
        {
            Model.Subscribe<AsteroidSpawnRequest>(OnSpawnRequest);
            Model.Subscribe<AsteroidDespawnRequest>(OnDespawnRequest);

            foreach (var v in ViewsContainer.GetViews<AsteroidView>())
            {
                Attach(v);
            }
        }

        public void Dispose()
        {
            Model.Unsubscribe<AsteroidSpawnRequest>(OnSpawnRequest);
            Model.Unsubscribe<AsteroidDespawnRequest>(OnDespawnRequest);

            foreach (var view in _attached.ToArray())
            {
                Detach(view);
            }
        }

        private void OnSpawnRequest()
        {
            if (!Model.TryGet(out AsteroidSpawnRequest request))
            {
                return;
            }
        
            var view = _pool.Spawn(request);
            Attach(view);
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
            
            Detach(asteroidView);
            _pool.Despawn(asteroidView);
        }

        private void Attach(BaseView view)
        {
            if (_attached.Add(view))
            {
                view.Emitted += OnEmitted;
            }
        }

        private void Detach(BaseView v)
        {
            if (_attached.Remove(v))
            {
                v.Emitted -= OnEmitted;
            }
        }
    }
}