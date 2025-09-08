using System.Collections.Generic;

namespace Runtime.Abstract.MVP
{
    public interface IViewsContainer
    {
        TView GetView<TView>() where TView : BaseView;
        List<TView> GetViews<TView>() where TView : BaseView;
        BaseView GetViewById(uint viewId);

        void AddView(BaseView view);
        void RemoveView(BaseView view);
    }
}