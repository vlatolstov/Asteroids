using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using Zenject;

namespace Runtime.Presenters
{
    public class WeaponPresenter : BasePresenter<GameModel>
    {
        private ProjectileView.Pool _pool;

        public WeaponPresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            ProjectileView.Pool pool) : base(model, viewsContainer, signalBus)
        {
            _pool = pool;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            ForwardOn<ProjectileShoot>();
            AddUnsub(Model.Subscribe<ProjectileShoot>(OnProjectileShoot));
        }

        private void OnProjectileShoot()
        {
            if (Model.TryGet(out ProjectileShoot shoot))
            {
                _pool.Spawn(shoot);
            }
        }
    }
}