using System;
using System.Collections.Generic;
using Runtime.Utils;
using Zenject;

namespace Runtime.Abstract.MVP
{
    public abstract class BasePresenter<TModel> : IInitializable, IDisposable where TModel : IModel
    {
        protected readonly TModel Model;
        protected readonly IViewsContainer ViewsContainer;

        private readonly Dictionary<BaseView, List<IDisposable>> _subsByView = new();

        protected BasePresenter(TModel model, IViewsContainer viewsContainer)
        {
            Model = model;
            ViewsContainer = viewsContainer;
        }

        private void SubscribeOn<TPayload>(BaseView view, Action<TPayload> onPayload) where TPayload : IData
        {
            var sub = view.Listen(onPayload);
            Track(view, sub);
        }

        private void Track(BaseView view, IDisposable disposableUnsub)
        {
            if (!_subsByView.TryGetValue(view, out var list))
            {
                list = new List<IDisposable>(8);
                _subsByView[view] = list;
            }

            list.Add(disposableUnsub);
        }

        protected void ForwardAllFrom(BaseView view)
        {
            if (!view)
            {
                return;
            }

            void ListenAllHandler(IData data)
            {
                if (data == null)
                {
                    return;
                }

                switch (data)
                {
                    case IStateData:
                        Model.ChangeData(data);
                        break;
                    case IEventData:
                        Model.Publish(data);
                        break;
                }
            }

            var sub = view.ListenAll(ListenAllHandler);
            Track(view, sub);
        }

        protected void ForwardOn<TPayload>(BaseView view, bool publish = false)
            where TPayload : IData
        {
            SubscribeOn<TPayload>(view, payload =>
            {
                if (publish)
                {
                    Model.Publish(payload);
                }
                else
                {
                    Model.ChangeData(payload);
                }
            });
        }

        protected void MutateOn<TState, TPayload>(BaseView view, Func<TState, TPayload, TState> mutate)
            where TState : IData
            where TPayload : IData
        {
            SubscribeOn<TPayload>(view, payload =>
                Model.ChangeData<TState>(prev => mutate(prev, payload)));
        }

        protected void Untrack(BaseView view)
        {
            if (!view)
            {
                return;
            }

            if (!_subsByView.TryGetValue(view, out var list))
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i]?.Dispose();
            }

            _subsByView.Remove(view);
        }

        protected void UntrackAll()
        {
            foreach (var list in _subsByView.Values)
            {
                foreach (var disposable in list)
                {
                    disposable?.Dispose();
                }
            }

            _subsByView.Clear();
        }

        public virtual void Initialize()
        { }

        public virtual void Dispose()
        {
            UntrackAll();
        }
    }

    public sealed class AnonDisposable : IDisposable
    {
        private Action _disposeAction;
        public AnonDisposable(Action disposeActionAction) => _disposeAction = disposeActionAction;

        public void Dispose()
        {
            _disposeAction?.Invoke();
            _disposeAction = null;
        }
    }
}