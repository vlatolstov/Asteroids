using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Runtime.Abstract.AssetManagement
{
    public abstract class LocalAssetLoader : IDisposable
    {
        private GameObject _cachedGameObject;

        protected async UniTask<T> LoadAsyncInternal<T>(string assetId)
        {
            var handle = Addressables.InstantiateAsync(assetId);
            _cachedGameObject = await handle.Task;

            if (!_cachedGameObject.TryGetComponent(out T component))
            {
                throw new NullReferenceException( $"Cant get component of type {typeof(T)} on {assetId}");
            }
            
            return component;
        }

        protected void UnloadInternal()
        {
            if (!_cachedGameObject)
            {
                return;
            }
            
            _cachedGameObject.SetActive(false);
            Addressables.ReleaseInstance(_cachedGameObject);
            _cachedGameObject = null;
        }

        public void Dispose()
        {
            UnloadInternal();
        }
    }
}