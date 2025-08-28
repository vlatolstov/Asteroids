using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.MVP
{
    public class ViewsContainer : IViewsContainer
    {
        private readonly Dictionary<Type, List<BaseView>> _views;

        public ViewsContainer(List<BaseView> views)
        {
            _views = views.GroupBy(v => v.GetType()).ToDictionary(g => g.Key, g => g.ToList());
        }

        public TView GetView<TView>() where TView : BaseView
        {
            return _views.ContainsKey(typeof(TView)) ? _views[typeof(TView)].Cast<TView>().FirstOrDefault() : null;
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
                if (list.Count == 0)
                {
                    _views.Remove(type);
                }
            }
        }
    }
}