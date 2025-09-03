using UnityEngine;
using Zenject;

namespace Runtime.Abstract.MVP
{
    public class BaseView : MonoBehaviour
    {
        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public uint ViewId { get; private set; } = 0;

        public void SetId(uint viewId) => ViewId = viewId;

        protected void Fire<TData>(TData data) where TData : IData
            => _signalBus.Fire(data);
    }
}