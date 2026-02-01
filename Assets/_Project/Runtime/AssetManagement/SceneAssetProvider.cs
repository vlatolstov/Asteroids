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
            var tasks = new List<UniTask<GameObject>>();
            foreach (var loader in _loaders)
            {
                if (_loadedGameObjects.ContainsKey(loader.Type))
                {
                    Debug.LogError($"Loader {loader.Type} has already been loaded");
                    continue;
                }

                var task = loader.LoadAsync();
                tasks.Add(task);
            }

            var result = new GameObject[tasks.Count];
            try
            {
                result = await UniTask.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (result.Length != _loaders.Count)
            {
                Debug.LogError($"There were {result.Length} GameObjects loaded");
                return;
            }
            
            for (var i = 0; i < result.Length; i++)
            {
                _loadedGameObjects.Add(_loaders[i].Type, result[i]);
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