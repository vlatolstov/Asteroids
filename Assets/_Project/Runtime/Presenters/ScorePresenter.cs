using System;
using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Settings;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class ScorePresenter : BasePresenter<GameModel>
    {
        private readonly AsteroidsModel _asteroidsModel;
        private readonly UfoModel _ufoModel;
        private readonly ScoreConfig _scoreConfig;

        public ScorePresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            AsteroidsModel asteroidsModel, UfoModel ufoModel, ScoreConfig scoreConfig)
            : base(model, viewsContainer, signalBus)
        {
            _asteroidsModel = asteroidsModel;
            _ufoModel = ufoModel;
            _scoreConfig = scoreConfig;
        }

        public override void Initialize()
        {
            AddUnsub(_ufoModel.Subscribe<UfoDestroyed>(OnUfoDestroyed));
            AddUnsub(_asteroidsModel.Subscribe<AsteroidDestroyed>(OnAsteroidDestroyed));
        }

        private void OnAsteroidDestroyed()
        {
            if (_asteroidsModel.TryGet(out AsteroidDestroyed signal))
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

        private void OnUfoDestroyed()
        {
            if (_ufoModel.TryGet(out UfoDestroyed signal))
            {
                Model.Publish(new ScoreAdded(_scoreConfig.UfoScore));
            }
        }
    }
}