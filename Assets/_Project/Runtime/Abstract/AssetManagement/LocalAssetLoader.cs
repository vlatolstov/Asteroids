using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Runtime.Abstract.AssetManagement
{
    public abstract class LocalAssetLoader<T> : IDisposable where T : MonoBehaviour
    {
        private GameObject _cachedGameObject;
        
        public T Asset { get; private set; }
        public bool Loaded { get; private set; } 
        
        protected abstract string AssetPath { get; }

        public async UniTask<T> LoadAsync()
        {
            var handle = Addressables.InstantiateAsync(AssetPath);
            _cachedGameObject = await handle.Task;

            if (!_cachedGameObject.TryGetComponent(out T component))
            {
                throw new NullReferenceException( $"Cant get component of type {typeof(T)} on {AssetPath}");
            }

            Loaded = true;
            Asset = component;
            return component;
        }

        public void Unload()
        {
            if (!_cachedGameObject)
            {
                return;
            }
            
            _cachedGameObject.SetActive(false);
            Addressables.ReleaseInstance(_cachedGameObject);
            _cachedGameObject = null;
            Loaded = false;
        }

        public void Dispose()
        {
            Unload();
        }
    }
}