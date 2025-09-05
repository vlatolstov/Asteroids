using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Runtime.Views;
using UnityEngine;
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

            AddUnsub(_shipModel.Subscribe<ShipPose>(OnPoseChanged));
        }

        public override void Dispose()
        {
            _hud = null;
        }

        private void OnPoseChanged()
        {
            if (!_hud)
            {
                return;
            }

            if (!_shipModel.TryGet(out ShipPose pose))
            {
                return;
            }

            _hud.SetPoseData(pose.Position, pose.Velocity, pose.AngleRadians);
        }
    }
}