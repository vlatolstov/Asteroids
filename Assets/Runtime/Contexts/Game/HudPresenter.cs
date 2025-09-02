using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class HudPresenter : BasePresenter<IModel>
    {
        private HudView _hud;

        public HudPresenter(IModel model, IViewsContainer views) : base(model, views)
        { }

        public override void Initialize()
        {
            base.Initialize();
            _hud = ViewsContainer.GetView<HudView>();
            
            if (!_hud)
            {
                Debug.LogError("HudView not found in container");
            }

            ForwardAllFrom(_hud);
            Model.Subscribe<ShipPose>(OnPoseChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            Model.Unsubscribe<ShipPose>(OnPoseChanged);
            _hud = null;
        }

        private void OnPoseChanged()
        {
            if (!_hud)
            {
                return;
            }

            if (!Model.TryGet(out ShipPose pose))
            {
                return;
            }

            _hud.SetPoseData(pose.Position, pose.Velocity, pose.AngleRadians);
        }
    }
}