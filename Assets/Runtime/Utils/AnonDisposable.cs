using System;

namespace Runtime.Utils
{
    public sealed class AnonDisposable : IDisposable
    {
        private Action _dispose;
        public AnonDisposable(Action d) => _dispose = d;
        public void Dispose() { _dispose?.Invoke(); _dispose = null; }
    }
}