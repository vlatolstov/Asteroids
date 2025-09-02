using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class HudPresenter : BasePresenter<IModel>, IInitializable, IDisposable
    {
        private HudView _hud;

        public HudPresenter(IModel model, IViewsContainer views) : base(model, views)
        { }

        public void Initialize()
        {
            _hud = ViewsContainer.GetView<HudView>();
            
            if (!_hud)
            {
                Debug.LogError("HudView not found in container");
            }

            _hud.Emitted += OnEmitted;
            Model.Subscribe<ShipPose>(OnPoseChanged);
            OnPoseChanged();
        }

        public void Dispose()
        {
            Model.Unsubscribe<ShipPose>(OnPoseChanged);
            _hud.Emitted -= OnEmitted;
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