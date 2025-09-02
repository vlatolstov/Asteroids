using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class InputPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private InputReceiverView _inputView;
        public InputPresenter(IModel model, IViewsContainer viewsContainer) : base(model, viewsContainer)
        { }

        public void Initialize()
        {
            _inputView = ViewsContainer.GetView<InputReceiverView>();
            _inputView.Emitted += OnEmitted;
            OnEmitted(new ThrustInput(0f));
            OnEmitted(new TurnInput(0f));
        }

        public void Dispose()
        {
            _inputView.Emitted -= OnEmitted;
            _inputView = null;
        }
    }
}