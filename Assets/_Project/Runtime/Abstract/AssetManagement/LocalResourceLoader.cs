using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Runtime.Abstract.AssetManagement
{
    public interface IResourceLoader : IDisposable
    {
        Type ResourceType { get; }
        string Key { get; }
        UniTask<ScriptableObject> LoadAsync();
    }

    public abstract class LocalResourceLoader<T> : IResourceLoader where T : ScriptableObject
    {
        private AsyncOperationHandle<T>? _handle;

        protected abstract string AssetPath { get; }

        public virtual string Key => AssetPath;

        public Type ResourceType => typeof(T);

        public async UniTask<ScriptableObject> LoadAsync()
        {
            if (_handle.HasValue)
            {
                return _handle.Value.Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(AssetPath);
            _handle = handle;
            return await handle.Task;
        }

        public void Dispose()
        {
            if (!_handle.HasValue)
            {
                return;
            }

            Addressables.Release(_handle.Value);
            _handle = null;
        }
    }
}
