using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class BackgroundPresenter : BasePresenter<ShipModel>
    {
        public BackgroundPresenter(ShipModel model, IViewsContainer viewsContainer, SignalBus signalBus) : base(model, viewsContainer, signalBus)
        { }

        public override void Initialize()
        {
            // Model.Subscribe<ShipPose>();
        }

        public void OnShipPoseChange()
        {
            
        }
    }
}