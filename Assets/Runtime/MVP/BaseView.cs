using System;
using UnityEngine;

namespace Runtime.MVP
{
    public class BaseView : MonoBehaviour
    {
        public event Action<IData> Emitted;

        protected void Emit<TData>(TData data) where TData : IData
            => Emitted?.Invoke(data);

        public void RemoveAllListeners() => Emitted = null;

        protected virtual void OnDestroy() { Emitted = null; }
    }
}