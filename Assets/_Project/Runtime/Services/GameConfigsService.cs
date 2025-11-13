using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Runtime.Abstract.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public interface IConfigsService : IDisposable
    {
        UniTask LoadAllAsync();
        T Get<T>(string key = null) where T : ScriptableObject;
    }

    public sealed class GameConfigsService : IConfigsService
    {
        private readonly IReadOnlyList<IConfigLoader> _loaders;
        private readonly Dictionary<Type, Dictionary<string, ScriptableObject>> _configs = new();
        private bool _loaded;

        public GameConfigsService(IEnumerable<IConfigLoader> loaders)
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
                    if (!_configs.TryGetValue(loader.ConfigType, out var dict))
                    {
                        dict = new Dictionary<string, ScriptableObject>();
                        _configs[loader.ConfigType] = dict;
                    }

                    dict[loader.Key] = so;
                }

                _loaded = true;
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception during configs async load. {ex}");
            }
        }

        public T Get<T>(string key = null) where T : ScriptableObject
        {
            if (!_loaded)
            {
                throw new InvalidOperationException("Configs not loaded yet.");
            }

            if (!_configs.TryGetValue(typeof(T), out var dict) || dict.Count == 0)
            {
                throw new KeyNotFoundException($"No configs of type {typeof(T).Name} are loaded.");
            }

            if (key == null)
            {
                if (dict.Count > 1)
                {
                    throw new InvalidOperationException(
                        $"Multiple configs of type {typeof(T).Name} are loaded. Specify a key.");
                }

                return (T)dict.Values.First();
            }

            if (dict.TryGetValue(key, out var so))
            {
                return (T)so;
            }

            throw new KeyNotFoundException($"Config {typeof(T).Name} with key '{key}' not found.");
        }

        public void Dispose()
        {
            foreach (var loader in _loaders)
            {
                loader.Dispose();
            }

            _configs.Clear();
        }
    }
}
