using System;
using System.Collections.Generic;

namespace Runtime.Abstract.MVP
{
    public interface IViewsContainer
    {
        public TView GetView<TView>() where TView : BaseView;
        public List<TView> GetViews<TView>() where TView : BaseView;
        
        public void AddView(BaseView view);
        public void RemoveView(BaseView view);

        event Action<BaseView> ViewAdded;
        event Action<BaseView> ViewRemoved;
    }
}