using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Runtime.Abstract.AssetManagement
{
    public abstract class PrefabAssetLoader<T> : IDisposable where T : Component
    {
        private AsyncOperationHandle<GameObject>? _handle;
        private GameObject _prefab;

        protected abstract string AssetPath { get; }

        public async UniTask<GameObject> LoadPrefabAsync()
        {
            if (_prefab)
            {
                return _prefab;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(AssetPath);
            _handle = handle;

            var prefab = await handle.Task;

            if (!prefab.TryGetComponent<T>(out _))
            {
                throw new NullReferenceException($"Prefab at path {AssetPath} does not contain component {typeof(T)}.");
            }

            _prefab = prefab;
            return _prefab;
        }

        public void Unload()
        {
            if (!_handle.HasValue)
            {
                return;
            }

            if (_prefab)
            {
                _prefab = null;
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
