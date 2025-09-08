using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using Zenject;

namespace Runtime.Presenters
{
    public class AnimationPresenter : BasePresenter<GameModel>
    {
        private readonly ShipModel _ship;
        private readonly AsteroidsModel _asteroids;
        private readonly UfoModel _ufo;

        private readonly AnimationView.Pool _pool;
        private readonly IGeneralVisualsConfig _general;

        public AnimationPresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipModel ship,
            AsteroidsModel asteroids, UfoModel ufo, IGeneralVisualsConfig general, AnimationView.Pool pool) : base(
            model, viewsContainer, signalBus)
        {
            _ship = ship;
            _asteroids = asteroids;
            _ufo = ufo;
            _general = general;
            _pool = pool;
        }

        public override void Initialize()
        {
            AddUnsub(Model.Subscribe<ProjectileHit>(OnProjectileHit));

            AddUnsub(_ship.Subscribe<ShipDestroyed>(OnShipDestroyed));

            AddUnsub(_ufo.Subscribe<UfoDestroyed>(OnUfoDestroyed));

            AddUnsub(_asteroids.Subscribe<AsteroidDestroyed>(OnAsteroidDestroyed));
        }

        private void OnProjectileHit()
        {
            if (Model.TryGet(out ProjectileHit hit) && hit.Projectile.HitAnimation)
            {
                _pool.Spawn(hit.Projectile.HitAnimation, hit.Position, hit.Projectile.Size);
            }
        }

        private void OnShipDestroyed()
        {
            if (_ship.TryGet(out ShipDestroyed dest))
            {
                _pool.Spawn(_general.ShipDestroyed, dest.Position, dest.Scale);
            }
        }

        private void OnUfoDestroyed()
        {
            if (_ufo.TryGet(out UfoDestroyed dest))
            {
                _pool.Spawn(_general.UfoDestroyed, dest.Position, dest.Scale);
            }
        }

        private void OnAsteroidDestroyed()
        {
            if (_asteroids.TryGet(out AsteroidDestroyed dest))
            {
                _pool.Spawn(_general.AsteroidDestroyed, dest.Position, dest.Scale);
            }
        }
    }
}