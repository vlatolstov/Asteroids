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
        private readonly SignalBus _signalBus;

        private readonly List<IDisposable> _subs = new();

        protected BasePresenter(TModel model, IViewsContainer viewsContainer, SignalBus signalBus)
        {
            Model = model;
            ViewsContainer = viewsContainer;
            _signalBus = signalBus;
        }

        protected void AddUnsub(IDisposable disposable)
        {
            _subs.Add(disposable);
        }
        
        private void SubscribeOn<TPayload>(Action<TPayload> onPayload) where TPayload : IData
        {
            _signalBus.Subscribe(onPayload);
            var sub = new AnonDisposable(() => _signalBus.TryUnsubscribe(onPayload));
            _subs.Add(sub);
        }

        protected void ForwardOn<TPayload>(bool publish = false)
            where TPayload : IData
        {
            SubscribeOn<TPayload>(payload =>
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

        protected void MutateOn<TState, TPayload>(Func<TState, TPayload, TState> mutate)
            where TState : IData
            where TPayload : IData
        {
            SubscribeOn<TPayload>(payload =>
                Model.ChangeData<TState>(prev => mutate(prev, payload)));
        }

        public virtual void Initialize()
        { }

        public virtual void Dispose()
        {
            foreach (var sub in _subs)
            {
                sub.Dispose();
            }

            _subs.Clear();
        }
    }
}