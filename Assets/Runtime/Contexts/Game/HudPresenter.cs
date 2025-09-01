using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
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
            AttachHud(ViewsContainer.GetView<HudView>());

            ViewsContainer.ViewAdded += OnViewAdded;
            ViewsContainer.ViewRemoved += OnViewRemoved;

            Model.Subscribe<ShipPose>(OnPoseChanged);

            PushPoseToHud();
        }

        public void Dispose()
        {
            ViewsContainer.ViewAdded -= OnViewAdded;
            ViewsContainer.ViewRemoved -= OnViewRemoved;
            Model.Unsubscribe<ShipPose>(OnPoseChanged);
            _hud = null;
        }

        private void OnViewAdded(BaseView view)
        {
            if (_hud == null && view is HudView hv)
                AttachHud(hv);
        }

        private void OnViewRemoved(BaseView view)
        {
            if (view == _hud)
                _hud = null;
        }

        private void AttachHud(HudView hv)
        {
            _hud = hv;
            PushPoseToHud();
        }

        private void OnPoseChanged() => PushPoseToHud();

        private void PushPoseToHud()
        {
            if (!_hud)
            {
                return;
            }

            if (!Model.TryGet(out ShipPose pose))
            {
                return;
            }

            _hud.SetPose(pose.Position, pose.Velocity, pose.AngleRadians);
        }
    }
}