using Runtime.Abstract.MVP;
using Runtime.Data;
using Runtime.Models;
using Zenject;

namespace Runtime.Presenters
{
    public class InputPresenter : BasePresenter<InputModel>
    {
        public InputPresenter(InputModel model, IViewsContainer viewsContainer, SignalBus signalBus) : base(model, viewsContainer, signalBus)
        { }

        public override void Initialize()
        {
            ForwardOn<ThrustInput>();
            ForwardOn<TurnInput>();
            ForwardOn<FireGunPressed>(publish: true);
            ForwardOn<AoeWeaponAttackPressed>(publish: true);
        }
    }
}