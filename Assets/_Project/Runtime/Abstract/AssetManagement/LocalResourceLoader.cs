using System;
using System.Collections.Generic;
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

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var locationsHandle = Addressables.LoadResourceLocationsAsync(AssetPath, typeof(T));
            try
            {
                var locations = await locationsHandle.Task;
                if (locations.Count == 0)
                {
                    Debug.LogWarning($"Addressables has no locations for key '{AssetPath}' ({typeof(T).Name}).");
                }

                var dependencyIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var location in locations)
                {
                    if (location.Dependencies == null || location.Dependencies.Count == 0)
                    {
                        continue;
                    }

                    foreach (var dependency in location.Dependencies)
                    {
                        if (dependency.InternalId.IndexOf(".bundle", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }

                        dependencyIds.Add(dependency.InternalId);
                    }
                }

                var dependencies = dependencyIds.Count == 0
                    ? "none"
                    : string.Join(", ", dependencyIds);
                Debug.Log($"Addressables key '{AssetPath}' ({typeof(T).Name}): dependencies=[{dependencies}]");
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
#endif

            var resource = await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded || !resource)
            {
                var details = handle.OperationException?.ToString() ?? "No exception details were provided.";
                Addressables.Release(handle);
                _handle = null;
                throw new InvalidOperationException(
                    $"Addressables failed to load key '{AssetPath}' ({typeof(T).Name}). {details}");
            }

            return resource;
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
