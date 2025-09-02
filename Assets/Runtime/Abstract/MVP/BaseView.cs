using System;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.Abstract.MVP
{
    public class BaseView : MonoBehaviour
    {
        public uint ViewId { get; private set; } = 0;

        public void SetId(uint viewId) => ViewId = viewId;
        public event Action<IData> Emitted;

        protected void Emit<TData>(TData data) where TData : IData
            => Emitted?.Invoke(data);

        public IDisposable Listen<TPayload>(Action<TPayload> on) where TPayload : IData
        {
            if (on == null) return new AnonDisposable(null);
            Emitted += Handler;
            return new AnonDisposable(() => Emitted -= Handler);

            void Handler(IData data)
            {
                if (data is TPayload payload)
                {
                    on(payload);
                }
            }
        }
        
        public IDisposable ListenAll(Action<IData> allDataHandler)
        {
            Emitted += allDataHandler;
            return new AnonDisposable(() => Emitted -= allDataHandler);
        }

        protected virtual void OnDestroy()
        {
            Emitted = null;
        }
    }
}