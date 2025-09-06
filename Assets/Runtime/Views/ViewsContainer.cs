using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Views
{
    public class ViewsContainer : IViewsContainer
    {
        private readonly Dictionary<Type, List<BaseView>> _views;
        private readonly Dictionary<uint, BaseView> _viewMap;

        private readonly Queue<uint> _availableId;
        private uint _lastId = 1;

        public ViewsContainer(List<BaseView> views)
        {
            _views = new Dictionary<Type, List<BaseView>>();
            _viewMap = new Dictionary<uint, BaseView>();
            _availableId = new Queue<uint>();

            foreach (var view in views)
            {
                AssignIdAndRegisterView(view);
                AddView(view);
            }
        }

        public BaseView GetViewById(uint viewId)
        {
            return _viewMap.GetValueOrDefault(viewId);
        }

        public TView GetView<TView>() where TView : BaseView
        {
            return _views
                .ContainsKey(typeof(TView))
                ? _views[typeof(TView)].Cast<TView>().FirstOrDefault()
                : null;
        }

        public List<TView> GetViews<TView>() where TView : BaseView
        {
            return _views.ContainsKey(typeof(TView)) ? _views[typeof(TView)].Cast<TView>().ToList() : new List<TView>();
        }

        public void AddView(BaseView view)
        {
            if (!view)
            {
                return;
            }

            var type = view.GetType();
            if (!_views.TryGetValue(type, out var list))
            {
                _views[type] = list = new List<BaseView>();
            }

            if (!list.Contains(view))
            {
                list.Add(view);
                AssignIdAndRegisterView(view);
                // Debug.Log($"{view} with {view.ViewId} id added in container");
            }
        }

        public void RemoveView(BaseView view)
        {
            if (!view)
            {
                return;
            }

            var type = view.GetType();
            if (_views.TryGetValue(type, out var list) && list.Remove(view))
            {
                UnregisterView(view);
                // Debug.Log($"{view} with {view.ViewId} id removed from container");

                if (list.Count == 0)
                {
                    _views.Remove(type);
                }
            }
        }

        private void AssignIdAndRegisterView(BaseView view)
        {
            if (!view)
            {
                return;
            }

            if (_viewMap.TryGetValue(view.ViewId, out var existing) && ReferenceEquals(existing, view))
            {
                return;
            }

            uint id;
            do
            {
                id = GetId();
            } while (_viewMap.ContainsKey(id));

            view.SetId(id);
            _viewMap[id] = view;
        }

        private void UnregisterView(BaseView view)
        {
            if (_viewMap.Remove(view.ViewId))
            {
                _availableId.Enqueue(view.ViewId);
            }

            view.SetId(0);
        }

        private uint GetId()
        {
            if (_availableId.Count == 0)
            {
                for (var i = 0; i < 100; i++)
                {
                    _availableId.Enqueue(_lastId++);
                }
            }

            return _availableId.Dequeue();
        }
    }
}