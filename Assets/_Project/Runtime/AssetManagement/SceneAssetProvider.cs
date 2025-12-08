using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.AssetManagement
{
    public class SceneAssetProvider : IDisposable
    {
        private List<IGameObjectLoader> _loaders = new();
        private Dictionary<Type, GameObject> _loadedGameObjects = new();

        public bool TryGetLoadedComponent<T>(out T component) where T : Component
        {
            if (!_loadedGameObjects.TryGetValue(typeof(T), out var gameObject))
            {
                component = default;
                Debug.LogError($"Unable to get component of type {typeof(T)}");
                return false;
            }

            if (!gameObject.TryGetComponent(out component))
            {
                component = default;
                Debug.LogError($"Cashed game object doesn't contains component of type {typeof(T)}");
                return false;
            }

            return true;
        }

        public async UniTask LoadAllAsync()
        {
            foreach (var loader in _loaders)
            {
                if (_loadedGameObjects.ContainsKey(loader.Type))
                {
                    Debug.LogError($"Loader {loader.Type} has already been loaded");
                    continue;
                }

                var asset = await loader.LoadAsync();
                _loadedGameObjects.Add(loader.Type, asset);
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