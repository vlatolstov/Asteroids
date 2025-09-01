using Zenject;

namespace Runtime.Abstract.MVP
{
    public abstract class ViewPool<TView> : MonoMemoryPool<TView> where TView : BaseView
    {
        private readonly IViewsContainer _viewsContainer;

        protected ViewPool(IViewsContainer viewsContainer)
        {
            _viewsContainer = viewsContainer;
        }
        
        protected override void OnSpawned(TView item)
        {
            base.OnSpawned(item);
            _viewsContainer.AddView(item);       
        }

        protected override void OnDespawned(TView item)
        {
            _viewsContainer.RemoveView(item);   
            base.OnDespawned(item);
        }
    }
}