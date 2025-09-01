using Runtime.Abstract.MVP;
using Runtime.Abstract.Weapons;
using Runtime.Data;
using Runtime.Settings;
using Runtime.Views;

namespace Runtime.Weapons
{
    public sealed class ProjectileHitResolver
    {
        private readonly IModel _model;
        private readonly ScoreConfig _scoreConfig;

        public ProjectileHitResolver(IModel model, ScoreConfig scoreConfig)
        {
            _model = model;
            _scoreConfig = scoreConfig;
        }

        public void Handle(in ProjectileHit hit)
        {
            if (hit.Source == Faction.Enemy && hit.Target.TryGetComponent<IPlayerTarget>(out _))
            {
                _model.ChangeData(new ShipDestroyed());
            }

            if (hit.Source == Faction.Player && hit.Target.TryGetComponent<IScorableTarget>(out _))
            {
                
                // _model.ChangeData(new ScoreAdded(scorable.OnHitScore));
            }
        }
    }
}