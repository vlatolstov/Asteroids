using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Runtime.Presenters
{
    public class HudPresenter : BasePresenter<GameModel>
    {
        private readonly ShipModel _shipModel;
        private HudView _hud;

        public HudPresenter(GameModel model, IViewsContainer viewsContainer, SignalBus signalBus,
            ShipModel shipModel) : base(model,
            viewsContainer, signalBus)
        {
            _shipModel = shipModel;
        }

        public override void Initialize()
        {
            _hud = ViewsContainer.GetView<HudView>();

            if (!_hud)
            {
                Debug.LogError("HudView not found in container");
            }

            _hud.UpdateGameState(GameState.Preparing);
            
            AddUnsub(_shipModel.Subscribe<ShipPose>(OnPoseChanged));
            
            AddUnsub(Model.Subscribe<AoeWeaponState>(OnAoeWeaponStateChanged));
            AddUnsub(Model.Subscribe<GameStateData>(OnGameStateChanged));
            AddUnsub(Model.Subscribe<TotalScore>(OnScoreChanged));
        }

        public override void Dispose()
        {
            _hud = null;
        }

        private void OnGameStateChanged()
        {
            if (Model.TryGet(out GameStateData state))
            {
                _hud.UpdateGameState(state.State);
            }
        }

        private void OnPoseChanged()
        {
            if (_shipModel.TryGet(out ShipPose pose))
            {
                _hud.UpdatePoseData(pose.Position, pose.Velocity, pose.AngleRadians);
            }
        }

        private void OnAoeWeaponStateChanged()
        {
            if (Model.TryGet(out AoeWeaponState state))
            {
                _hud.UpdateLaserData(state.MaxCharges, state.Charges, state.RechargeRatio);
            }
        }

        private void OnScoreChanged()
        {
            if (Model.TryGet(out TotalScore score))
            {
                _hud.UpdateScore(score.Amount);
            }
        }
    }
}