using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class InputPresenter : BasePresenter<IModel>
    {
        private InputReceiverView _inputView;
        public InputPresenter(IModel model, IViewsContainer viewsContainer) : base(model, viewsContainer)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _inputView = ViewsContainer.GetView<InputReceiverView>();
            ForwardAllFrom(_inputView);
        }

        public override void Dispose()
        {
            base.Dispose();
            _inputView = null;
        }
    }
}