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
    }
}