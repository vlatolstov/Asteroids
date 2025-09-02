using System;

namespace Runtime.Abstract.MVP
{
    public interface IModel
    {
        void Subscribe<TData>(Action action) where TData : IData;
        void Unsubscribe<TData>(Action action) where TData : IData;
        void ChangeData<TData>(Func<TData, TData> mutate) where TData : IData;
        void ChangeData(IData newValue);
        void Publish(IData eventData);
        public void Publish<TEvent>(TEvent eventData) where TEvent : IData;
        bool TryGet<TData>(out TData data) where TData : IData;
    }
}