using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using Zenject;

namespace Runtime.Presenters
{
    public class AudioPresenter : BasePresenter<GameModel>
    {
        private readonly ShipModel _shipModel;
        
        private readonly AudioSourceView.Pool _audioPool;
        private readonly IGeneralSoundsConfig _generalSounds;

        public AudioPresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipModel shipModel, IGeneralSoundsConfig generalSounds, AudioSourceView.Pool audioPool) : base(model,
            viewsContainer, signalBus)
        {
            _shipModel = shipModel;
            _generalSounds = generalSounds;
            _audioPool = audioPool;
        }

        public override void Initialize()
        {
            AddUnsub(Model.Subscribe<ProjectileShoot>(OnProjectileShoot));
            AddUnsub(Model.Subscribe<ProjectileHit>(OnProjectileHit));
            
            AddUnsub(Model.Subscribe<AoeAttackReleased>(OnAoeAttackReleased));
            AddUnsub(Model.Subscribe<AoeHit>(OnAoeHit));
            
            AddUnsub(_shipModel.Subscribe<ShipSpawned>(OnShipSpawned));
        }

        private void OnProjectileShoot()
        {
            if (Model.TryGet(out ProjectileShoot shoot))
            {
                _audioPool.Spawn(shoot.Position, shoot.Weapon.AttackSound);
            }
        }
        
        private void OnProjectileHit()
        {
            if (Model.TryGet(out ProjectileHit hit))
            {
                _audioPool.Spawn(hit.Position, hit.Projectile.HitSound);
            }
        }
        

        private void OnAoeAttackReleased()
        {
            if (Model.TryGet(out AoeAttackReleased attack))
            {
                _audioPool.Spawn(attack.Emitter.position, attack.Weapon.AttackSound);
            }
        }

        private void OnAoeHit()
        {
            if (Model.TryGet(out AoeHit hit))
            {
                _audioPool.Spawn(hit.Position, hit.Attack.HitSound);
            }
        }

        private void OnShipSpawned()
        {
            if (_shipModel.TryGet(out ShipSpawned spawn))
            {
                _audioPool.Spawn(spawn.Position, _generalSounds.ShipSpawn);
            }
        }
    }
}