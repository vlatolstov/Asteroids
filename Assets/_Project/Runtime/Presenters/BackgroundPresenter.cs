using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using _Project.Runtime.Views;
using Zenject;

namespace _Project.Runtime.Presenters
{
    public class BackgroundPresenter : BasePresenter<ShipModel>
    {
        private readonly BackgroundView _bg;

        public BackgroundPresenter(ShipModel model, IViewsContainer viewsContainer, SignalBus signalBus) : base(model,
            viewsContainer, signalBus)
        {
            _bg = viewsContainer.GetView<BackgroundView>();
        }

        public override void Initialize()
        {
            AddUnsub(Model.Subscribe<ShipPose>(OnShipPoseChange));
        }

        public void OnShipPoseChange()
        {
            if (Model.TryGet(out ShipPose pose))
            {
                _bg.SetPlayerVelocity(pose.Velocity);
            }
        }
    }
}