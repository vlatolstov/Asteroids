using System;

namespace Runtime.Abstract.MVP
{
    public abstract class BasePresenter<TModel> where TModel : IModel
    {
        protected readonly TModel Model;
        protected readonly IViewsContainer ViewsContainer;
        
        protected BasePresenter(TModel model, IViewsContainer viewsContainer)
        {
            Model = model;
            ViewsContainer = viewsContainer;
        }
        
        protected void OnEmitted(IData data)
        {
            Model.ChangeData(data);
        }

        protected void OnEmitted<TData>(Func<TData, TData> mutate) where TData : IData
        {
            Model.ChangeData(mutate);
        }
    }
}