using System;

namespace _Project.Runtime.Utils
{
    public sealed class AnonDisposable : IDisposable
    {
        private Action _disposeAction;
        public AnonDisposable(Action disposeActionAction) => _disposeAction = disposeActionAction;

        public void Dispose()
        {
            _disposeAction?.Invoke();
            _disposeAction = null;
        }
    }
}