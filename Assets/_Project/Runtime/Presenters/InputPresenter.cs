using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using Zenject;

namespace _Project.Runtime.Presenters
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