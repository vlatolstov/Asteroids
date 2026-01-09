using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Runtime.Abstract.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public interface IResourcesService : IDisposable
    {
        UniTask LoadAllAsync();
        T Get<T>(string key = null) where T : ScriptableObject;
    }

    public sealed class GameResourcesService : IResourcesService
    {
        private readonly IReadOnlyList<IResourceLoader> _loaders;
        private readonly Dictionary<Type, Dictionary<string, ScriptableObject>> _resources = new();
        private bool _loaded;

        public GameResourcesService(IEnumerable<IResourceLoader> loaders)
        {
            _loaders = loaders.ToList();
        }

        public async UniTask LoadAllAsync()
        {
            if (_loaded)
            {
                await UniTask.CompletedTask;
            }
            
            await LoadInternalAsync();
        }

        private async UniTask LoadInternalAsync()
        {
            try
            {
                foreach (var loader in _loaders)
                {
                    var so = await loader.LoadAsync();
                    if (!_resources.TryGetValue(loader.ResourceType, out var dict))
                    {
                        dict = new Dictionary<string, ScriptableObject>();
                        _resources[loader.ResourceType] = dict;
                    }

                    dict[loader.Key] = so;
                }

                _loaded = true;
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception during resources async load. {ex}");
            }
        }

        public T Get<T>(string key = null) where T : ScriptableObject
        {
            if (!_loaded)
            {
                throw new InvalidOperationException("Resources not loaded yet.");
            }

            if (!_resources.TryGetValue(typeof(T), out var dict) || dict.Count == 0)
            {
                throw new KeyNotFoundException($"No resources of type {typeof(T).Name} are loaded.");
            }

            if (key == null)
            {
                if (dict.Count > 1)
                {
                    throw new InvalidOperationException(
                        $"Multiple resources of type {typeof(T).Name} are loaded. Specify a key.");
                }

                return (T)dict.Values.First();
            }

            if (dict.TryGetValue(key, out var so))
            {
                return (T)so;
            }

            throw new KeyNotFoundException($"Resource {typeof(T).Name} with key '{key}' not found.");
        }

        public void Dispose()
        {
            foreach (var loader in _loaders)
            {
                loader.Dispose();
            }

            _resources.Clear();
        }
    }
}
