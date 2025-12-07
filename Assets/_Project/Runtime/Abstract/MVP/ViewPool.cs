using System;
using System.Collections.Generic;
using _Project.Runtime.Tools;
using UnityEngine;

namespace _Project.Runtime.Abstract.MVP
{
    public abstract class ViewPoolBase<TView> where TView : BaseView
    {
        private readonly Stack<TView> _items = new();
        private readonly Func<TView> _factory;
        private readonly Transform _parent;
        private readonly ReusableIdContainer _idContainer = new();

        protected ViewPoolBase(Func<TView> factory, Transform parent, int warmup)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _parent = parent;

            Warmup(warmup);
        }

        protected TView SpawnInternal(Action<TView> reinitialize)
        {
            var item = _items.Count > 0 ? _items.Pop() : CreateItem();

            item.SetId(_idContainer.Acquire());
            item.gameObject.SetActive(true);
            item.transform.SetParent(_parent, true);

            reinitialize?.Invoke(item);

            OnSpawned(item);
            return item;
        }

        protected void DespawnInternal(TView item)
        {
            if (!item)
            {
                return;
            }

            OnDespawned(item);
            _idContainer.Release(item.ViewId);

            item.transform.SetParent(_parent, true);
            item.gameObject.SetActive(false);
            _items.Push(item);
        }

        protected virtual void OnSpawned(TView item)
        { }

        protected virtual void OnDespawned(TView item)
        { }

        private TView CreateItem()
        {
            var instance = _factory();
            instance.transform.SetParent(_parent, false);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void Warmup(int warmup)
        {
            for (int i = 0; i < warmup; i++)
            {
                var item = CreateItem();
                _items.Push(item);
            }
        }
    }

    public abstract class ViewPool<TView> : ViewPoolBase<TView> where TView : BaseView
    {
        protected ViewPool(Func<TView> factory, Transform parent, int warmup = 0)
            : base(factory, parent, warmup)
        { }

        public TView Spawn()
        {
            return SpawnInternal(Reinitialize);
        }

        public void Despawn(TView item)
        {
            DespawnInternal(item);
        }

        protected virtual void Reinitialize(TView item)
        { }
    }

    public abstract class ViewPool<TParam1, TView> : ViewPoolBase<TView>
        where TView : BaseView
    {
        protected ViewPool(Func<TView> factory, Transform parent, int warmup = 0)
            : base(factory, parent, warmup)
        { }

        public TView Spawn(TParam1 param1)
        {
            return SpawnInternal(item => Reinitialize(param1, item));
        }

        public void Despawn(TView item)
        {
            DespawnInternal(item);
        }

        protected abstract void Reinitialize(TParam1 param1, TView item);
    }

    public abstract class ViewPool<TParam1, TParam2, TView> : ViewPoolBase<TView>
        where TView : BaseView
    {
        protected ViewPool(Func<TView> factory, Transform parent, int warmup = 0)
            : base(factory, parent, warmup)
        { }

        public TView Spawn(TParam1 param1, TParam2 param2)
        {
            return SpawnInternal(item => Reinitialize(param1, param2, item));
        }

        public void Despawn(TView item)
        {
            DespawnInternal(item);
        }

        protected abstract void Reinitialize(TParam1 param1, TParam2 param2, TView item);
    }

    public abstract class ViewPool<TParam1, TParam2, TParam3, TView> : ViewPoolBase<TView>
        where TView : BaseView
    {
        protected ViewPool(Func<TView> factory, Transform parent, int warmup = 0)
            : base(factory, parent, warmup)
        { }

        public TView Spawn(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return SpawnInternal(item => Reinitialize(param1, param2, param3, item));
        }

        public void Despawn(TView item)
        {
            DespawnInternal(item);
        }

        protected abstract void Reinitialize(TParam1 param1, TParam2 param2, TParam3 param3, TView item);
    }

    public abstract class ViewPool<TParam1, TParam2, TParam3, TParam4, TView> : ViewPoolBase<TView>
        where TView : BaseView
    {
        protected ViewPool(Func<TView> factory, Transform parent, int warmup = 0)
            : base(factory, parent, warmup)
        { }

        public TView Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return SpawnInternal(item => Reinitialize(param1, param2, param3, param4, item));
        }

        public void Despawn(TView item)
        {
            DespawnInternal(item);
        }

        protected abstract void Reinitialize(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4,
            TView item);
    }
}
