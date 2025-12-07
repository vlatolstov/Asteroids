using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Runtime.AssetManagement
{
    public class LocalAssetProvider : IDisposable
    {
        private Dictionary<string, GameObject> _cachedGameObjects = new();

        public bool TryGetLoadedAsset<T>(string path, out T loadedAsset)
        {
            if (!_cachedGameObjects.TryGetValue(path, out var go))
            {
                Debug.LogError($"{typeof(T)} on {path} not loaded yet");
                loadedAsset = default;
                return false;
            }

            if (!go.TryGetComponent(out loadedAsset))
            {
                Debug.LogError($"Cant get component of type {typeof(T)} on {path}");
                loadedAsset = default;
                return false;
            }

            return true;
        }

        public async UniTask LoadAsync(string path)
        {
            if (!_cachedGameObjects.ContainsKey(path))
            {
                var handle = Addressables.InstantiateAsync(path);
                var go = await handle.Task;
                _cachedGameObjects.Add(path, go);
            }
        }

        public void Unload(string path)
        {
            if (_cachedGameObjects != null &&
                UnloadInternal(path))
            {
                _cachedGameObjects.Remove(path);
            }
        }

        public void Dispose()
        {
            foreach (string path in _cachedGameObjects.Keys)
            {
                UnloadInternal(path);
            }

            _cachedGameObjects.Clear();
            _cachedGameObjects = null;
        }

        private bool UnloadInternal(string path)
        {
            if (!_cachedGameObjects.TryGetValue(path, out var go))
            {
                return false;
            }

            if (go)
            {
                go.SetActive(false);
                Addressables.ReleaseInstance(go);
            }

            return true;
        }
    }
}