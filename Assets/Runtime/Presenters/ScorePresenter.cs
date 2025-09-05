using System;
using Runtime.Abstract.Configs;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Zenject;

namespace Runtime.Presenters
{
    public class ScorePresenter : BasePresenter<GameModel>
    {
        private ShipModel _shipModel;
        private IScoreConfig _scoreConfig;

        public ScorePresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus, ShipModel shipModel)
            : base(model, viewsContainer, signalBus)
        {
            _shipModel = shipModel;
        }

        public override void Initialize()
        {
            AddUnsub(_shipModel.Subscribe<AsteroidDestroyed>(OnAsteroidDestroyed));
            MutateOn<TotalScore, ScoreAdded>(OnScoreAdded);
        }

        private TotalScore OnScoreAdded(TotalScore curScore, ScoreAdded added)
        {
            return new TotalScore(curScore.Amount + added.Amount);
        }

        private void OnAsteroidDestroyed()
        {
            if (_shipModel.TryGet(out AsteroidDestroyed signal))
            {
                var scoreAdded = signal.Size switch
                {
                    AsteroidSize.Large => new ScoreAdded(_scoreConfig.LargeAsteroidScore),
                    AsteroidSize.Small => new ScoreAdded(_scoreConfig.SmallAsteroidScore),
                    _ => throw new Exception("Unknown asteroid size")
                };

                Model.Publish(scoreAdded);
            }
        }
    }
}