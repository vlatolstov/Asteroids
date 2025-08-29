using System;
using System.Collections.Generic;
using Runtime.Abstract.MVP;
using Runtime.Models;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class ShipPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private readonly HashSet<ShipView> _attached = new();

        public ShipPresenter(IModel model, IViewsContainer viewsContainer) : base(model, viewsContainer)
        { }

        public void Initialize()
        {
            Debug.Log($"ShipPresenter.Initialized");
            foreach (var shipView in ViewsContainer.GetViews<ShipView>())
            {
                Debug.Log($"{shipView} attached");
                Attach(shipView);
            }
            
            ViewsContainer.ViewAdded += ViewAdded;
            ViewsContainer.ViewRemoved += ViewRemoved;
        }

        public void Dispose()
        {
            foreach (var shipView in _attached)
            {
                shipView.Emitted -= OnViewEmitted;
            }

            _attached.Clear();
        }

        private void Attach(ShipView shipView)
        {
            if (!_attached.Add(shipView))
            {
                return;
            }

            shipView.Emitted += OnViewEmitted;
        }

        private void Detach(ShipView shipView)
        {
            if (_attached.Remove(shipView))
            {
                shipView.Emitted -= OnViewEmitted;
            }
        }

        private void ViewAdded(BaseView view)
        {
            if (view is ShipView sv)
            {
                Attach(sv);
            }
        }

        private void ViewRemoved(BaseView view)
        {
            if (view is ShipView sv)
            {
                Detach(sv);
            }
        }


        private void OnViewEmitted(IData data)
        {
            Model.ChangeData(data);
            Debug.Log("ShipPresenter.ViewEmitted");
        }
    }
}