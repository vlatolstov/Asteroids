using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.AssetManagement
{
    public class SceneAssetProvider : IDisposable
    {
        private List<IGameObjectLoader> _loaders = new();
        private Dictionary<string, GameObject> _loadedGameObjects = new();

        public bool TryGetLoadedComponent<T>(string assetKey, out T component) where T : Component
        {
            if (string.IsNullOrWhiteSpace(assetKey))
            {
                component = default;
                Debug.LogError("Asset key is null or empty.");
                return false;
            }

            if (!_loadedGameObjects.TryGetValue(assetKey, out var gameObject))
            {
                component = default;
                Debug.LogError($"Unable to get asset with key '{assetKey}' and component type {typeof(T)}");
                return false;
            }

            if (!gameObject.TryGetComponent(out component))
            {
                component = default;
                Debug.LogError(
                    $"Cached game object for key '{assetKey}' does not contain component of type {typeof(T)}");
                return false;
            }

            return true;
        }

        public async UniTask LoadAllAsync()
        {
            var tasks = new List<UniTask<GameObject>>();
            var loadingLoaders = new List<IGameObjectLoader>();
            var loadingKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var loader in _loaders)
            {
                if (_loadedGameObjects.ContainsKey(loader.AssetKey))
                {
                    Debug.LogError($"Loader '{loader.AssetKey}' has already been loaded");
                    continue;
                }

                if (!loadingKeys.Add(loader.AssetKey))
                {
                    Debug.LogError($"Loader '{loader.AssetKey}' has already been scheduled for loading");
                    continue;
                }

                var task = loader.LoadAsync();
                tasks.Add(task);
                loadingLoaders.Add(loader);
            }

            GameObject[] result;
            try
            {
                result = await UniTask.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            if (result.Length != loadingLoaders.Count)
            {
                Debug.LogError($"There were {result.Length} GameObjects loaded");
                return;
            }
            
            for (var i = 0; i < result.Length; i++)
            {
                _loadedGameObjects.Add(loadingLoaders[i].AssetKey, result[i]);
            }
        }

        public void RegisterLoader(IGameObjectLoader loader)
        {
            _loaders.Add(loader);
        }

        public void Dispose()
        {
            foreach (var loader in _loaders)
            {
                loader.Unload();
            }

            _loaders.Clear();
            _loaders = null;
            _loadedGameObjects.Clear();
            _loadedGameObjects = null;
        }
    }
}
