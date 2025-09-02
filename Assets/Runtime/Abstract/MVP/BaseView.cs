using System;
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

        public void RemoveAllListeners() => Emitted = null;

        protected virtual void OnDestroy() { Emitted = null; }
    }
}