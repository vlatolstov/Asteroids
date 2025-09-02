using System;
using System.Collections.Generic;

namespace Runtime.Abstract.MVP
{
    public abstract class BaseModel : IModel
    {
        private readonly Dictionary<Type, IData> _dataContainer = new();
        private readonly Dictionary<Type, List<Action>> _subscriptions = new();

        public void Subscribe<TData>(Action action) where TData : IData
        {
            var type = typeof(TData);
            if (!_subscriptions.TryGetValue(type, out var list))
            {
                list = new List<Action>();
                _subscriptions[type] = list;
            }

            if (action != null && !list.Contains(action))
            {
                list.Add(action);
            }
        }

        public void Unsubscribe<TData>(Action action) where TData : IData
        {
            var type = typeof(TData);
            if (!_subscriptions.TryGetValue(type, out var list))
            {
                return;
            }

            list.Remove(action);
            if (list.Count == 0)
            {
                _subscriptions.Remove(type);
            }
        }

        public void ChangeData<TData>(Func<TData, TData> mutate) where TData : IData
        {
            if (mutate == null) return;

            var type = typeof(TData);
            _dataContainer.TryGetValue(type, out var oldObj);
            var before = oldObj is TData ok ? ok : default;

            var after = mutate(before);

            if (Equals(before, after)) return;


            if (after is null)
                _dataContainer.Remove(type);
            else
                _dataContainer[type] = after;

            OnDataChange(after);
            Notify(type);
        }

        public void ChangeData(IData newValue)
        {
            if (newValue == null)
            {
                return;
            }

            var type = newValue.GetType();
            if (_dataContainer.TryGetValue(type, out var oldValue) && Equals(oldValue, newValue))
            {
                return;
            }

            OnDataChange(newValue);
            _dataContainer[type] = newValue;
            Notify(type);
        }

        public void Publish(IData eventData)
        {
            if (eventData == null) return;

            var type = eventData.GetType();

            _dataContainer[type] = eventData;

            try
            {
                OnEventPublished(eventData);
                Notify(type);
            }
            finally
            {
                
                if (_dataContainer.TryGetValue(type, out var current) && ReferenceEquals(current, eventData))
                {
                    _dataContainer.Remove(type);
                }
            }
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : IData
        {
            Publish((IData)eventData);
        }

        public bool TryGet<TData>(out TData data) where TData : IData
        {
            if (_dataContainer.TryGetValue(typeof(TData), out var v) && v is TData t)
            {
                data = t;
                return true;
            }

            data = default;
            return false;
        }

        private void Notify(Type type)
        {
            if (!_subscriptions.TryGetValue(type, out var listeners))
            {
                return;
            }

            foreach (var action in listeners)
            {
                OnNotify(action);
                action?.Invoke();
            }
        }

        protected virtual void OnDataChange(IData newValue)
        { }

        protected virtual void OnEventPublished(IData eventData)
        { }

        protected virtual void OnNotify(Action action)
        { }
    }
}