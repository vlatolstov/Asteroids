using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using Zenject;

namespace Runtime.Presenters
{
    public class WeaponPresenter : BasePresenter<GameModel>
    {
        private readonly ProjectileView.Pool _projectilePool;
        private readonly AoeAttackView.Pool _aoePool;

        public WeaponPresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            ProjectileView.Pool projectilePool, AoeAttackView.Pool aoePool) : base(model, viewsContainer, signalBus)
        {
            _projectilePool = projectilePool;
            _aoePool = aoePool;
        }

        public override void Initialize()
        {
            base.Initialize();

            ForwardOn<AoeWeaponState>();
            
            ForwardOn<ProjectileShoot>(publish: true);
            ForwardOn<ProjectileHit>(publish: true);
            
            ForwardOn<AoeAttackReleased>(publish: true);
            ForwardOn<AoeHit>(publish: true);
            
            AddUnsub(Model.Subscribe<ProjectileShoot>(OnProjectileShoot));
            AddUnsub(Model.Subscribe<AoeAttackReleased>(OnAoeAttackReleased));
        }

        private void OnProjectileShoot()
        {
            if (Model.TryGet(out ProjectileShoot shoot))
            {
                _projectilePool.Spawn(shoot);
            }
        }

        private void OnAoeAttackReleased()
        {
            if (Model.TryGet(out AoeAttackReleased shoot))
            {
                _aoePool.Spawn(shoot.Origin, shoot.Weapon);
            }
        }
    }
}