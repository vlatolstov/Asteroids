using System;
using System.Collections.Generic;
using Runtime.Abstract.Movement;
using Runtime.Abstract.MVP;
using Runtime.Views;
using Runtime.Data;
using Zenject;

namespace Runtime.Contexts.Asteroids
{
    public class AsteroidPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private readonly AsteroidLargeView.Pool _largePool;
        private readonly AsteroidSmallView.Pool _smallPool;

        private readonly Dictionary<int, BaseView> _live = new();
        private readonly Dictionary<BaseView, Action<IData>> _handlers = new();

        public AsteroidPresenter(
            IModel model, IViewsContainer views,
            AsteroidLargeView.Pool largePool, AsteroidSmallView.Pool smallPool)
            : base(model, views)
        {
            _largePool = largePool;
            _smallPool = smallPool;
        }

        public void Initialize()
        {
            Model.Subscribe<AsteroidSpawnRequest>(OnSpawnRequest);
            Model.Subscribe<AsteroidDespawnRequest>(OnDespawnRequest);

            foreach (var v in ViewsContainer.GetViews<AsteroidLargeView>())
            {
                Attach(v);
            }

            foreach (var v in ViewsContainer.GetViews<AsteroidSmallView>())
            {
                Attach(v);
            }

            ViewsContainer.ViewAdded += OnViewAdded;
            ViewsContainer.ViewRemoved += OnViewRemoved;
        }

        public void Dispose()
        {
            ViewsContainer.ViewAdded -= OnViewAdded;
            ViewsContainer.ViewRemoved -= OnViewRemoved;
            Model.Unsubscribe<AsteroidSpawnRequest>(OnSpawnRequest);
            Model.Unsubscribe<AsteroidDespawnRequest>(OnDespawnRequest);

            foreach (var kv in _handlers)
            {
                kv.Key.Emitted -= kv.Value;
            }

            _handlers.Clear();
            _live.Clear();
        }

        private void OnSpawnRequest()
        {
            if (!Model.TryGet(out AsteroidSpawnRequest cmd))
            {
                return;
            }

            if (cmd.Size == AsteroidSize.Large)
            {
                var v = _largePool.Spawn();
                SetupSpawned(v, cmd);
            }
            else
            {
                var v = _smallPool.Spawn();
                SetupSpawned(v, cmd);
            }
        }

        private void SetupSpawned(BaseView view, AsteroidSpawnRequest cmd)
        {
            view.GetComponent<IMove>()?.SetPose(cmd.Pos, cmd.Vel, cmd.AngleRad);
            view.GetComponent<IMotorInput>()?.SetControls(1f, 0f);

            var ast = view as BaseAsteroidView;
            ast?.SetId(cmd.Id);

            _live[cmd.Id.Value] = view;
            Attach(view);
        }

        private void OnDespawnRequest()
        {
            if (!Model.TryGet(out AsteroidDespawnRequest cmd))
            {
                return;
            }

            if (!_live.Remove(cmd.Id.Value, out var view))
            {
                return;
            }

            Detach(view);

            if (view is AsteroidLargeView lv)
            {
                _largePool.Despawn(lv);
            }
            else if (view is AsteroidSmallView sv)
            {
                _smallPool.Despawn(sv);
            }
            else
            {
                throw new InvalidOperationException($"Invalid view type {view.GetType()}");
            }
        }

        private void OnViewAdded(BaseView view)
        {
            switch (view)
            {
                case AsteroidLargeView lv:
                    Attach(lv);
                    break;
                case AsteroidSmallView sv:
                    Attach(sv);
                    break;
            }
        }

        private void OnViewRemoved(BaseView view) => Detach(view);

        private void Attach(BaseView v)
        {
            if (_handlers.ContainsKey(v))
            {
                return;
            }
            void Handler(IData d) => Model.ChangeData(d); 
            _handlers.Add(v, Handler);
            v.Emitted += Handler;
        }

        private void Detach(BaseView v)
        {
            if (_handlers.Remove(v, out var h))
            {
                v.Emitted -= h;
            }
        }
    }
}