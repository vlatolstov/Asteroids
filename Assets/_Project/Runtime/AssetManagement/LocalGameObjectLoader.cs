using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Runtime.AssetManagement
{
    public interface IGameObjectLoader
    {
        Type Type { get; }
        UniTask<GameObject> LoadAsync();
        void Unload();
    }

    public class LocalGameObjectLoader<TComponent> : IDisposable, IGameObjectLoader where TComponent : Component
    {
        public Type Type => typeof(TComponent);

        private readonly string _path;
        private readonly bool _createInstance;
        private GameObject _cachedGameObject;
        private AsyncOperationHandle<GameObject>? _handle;

        public LocalGameObjectLoader(string assetPath, bool createInstance = false)
        {
            _path = assetPath;
            _createInstance = createInstance;
        }

        public async UniTask<GameObject> LoadAsync()
        {
            if (_cachedGameObject)
            {
                return _cachedGameObject;
            }

            var handle = _createInstance
                ? Addressables.InstantiateAsync(_path)
                : Addressables.LoadAssetAsync<GameObject>(_path);
            _handle = handle;

            var gameObject = await handle.Task;

            if (!gameObject.TryGetComponent<TComponent>(out _))
            {
                throw new NullReferenceException(
                    $"GameObject at path {_path} does not contain component {typeof(TComponent)}.");
            }

            _cachedGameObject = gameObject;
            return _cachedGameObject;
        }

        public void Unload()
        {
            if (!_handle.HasValue)
            {
                return;
            }

            if (_cachedGameObject)
            {
                _cachedGameObject = null;
            }

            Addressables.Release(_handle.Value);
            _handle = null;
        }

        public void Dispose()
        {
            Unload();
        }
    }
}