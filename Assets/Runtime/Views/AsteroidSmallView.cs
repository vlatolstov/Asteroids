using Runtime.Abstract.MVP;

namespace Runtime.Views
{
    public class AsteroidSmallView : BaseAsteroidView
    {
        public class Pool : ViewPool<BaseAsteroidView>
        {
            public Pool(IViewsContainer viewsContainer) : base(viewsContainer)
            { }
        }
    }
}