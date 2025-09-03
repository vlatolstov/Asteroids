using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Views;
using Zenject;

namespace Runtime.Contexts.Game
{
    public class InputPresenter : BasePresenter<IModel>
    {
        public InputPresenter(IModel model, IViewsContainer viewsContainer, SignalBus signalBus) : base(model,
            viewsContainer, signalBus)
        { }

        public override void Initialize()
        {
            base.Initialize();

            ForwardOn<ThrustInput>();
            ForwardOn<TurnInput>();
            ForwardOn<FireBulletPressed>(publish: true);
            ForwardOn<FireLaserPressed>(publish: true);
            ForwardOn<ShipSpawnRequest>(publish: true);
            ForwardOn<ShipDespawnRequest>(publish: true);
        }
    }
}