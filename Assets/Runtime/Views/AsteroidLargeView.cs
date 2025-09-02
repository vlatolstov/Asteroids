using Runtime.Abstract.MVP;

namespace Runtime.Views
{
    public class AsteroidLargeView : BaseAsteroidView
    {
        public class Pool : ViewPool<BaseAsteroidView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }
            
            protected override void OnDespawned(BaseAsteroidView item)
            {
                item.Reinitialize();
                base.OnDespawned(item);
            }
        }
    }
}