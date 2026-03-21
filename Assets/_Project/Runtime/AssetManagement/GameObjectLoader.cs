using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Runtime.AssetManagement
{
    public interface IGameObjectLoader
    {
        string AssetKey { get; }
        UniTask<GameObject> LoadAsync();
        void Unload();
    }

    public class GameObjectLoader<TComponent> : IDisposable, IGameObjectLoader where TComponent : Component
    {
        public string AssetKey { get; }

        private readonly bool _createInstance;
        private GameObject _cachedGameObject;
        private AsyncOperationHandle<GameObject>? _handle;

        public GameObjectLoader(string assetPath, bool createInstance = false)
        {
            AssetKey = assetPath;
            _createInstance = createInstance;
        }

        public async UniTask<GameObject> LoadAsync()
        {
            if (_cachedGameObject)
            {
                return _cachedGameObject;
            }

            var handle = _createInstance
                ? Addressables.InstantiateAsync(AssetKey)
                : Addressables.LoadAssetAsync<GameObject>(AssetKey);
            _handle = handle;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var locationsHandle = Addressables.LoadResourceLocationsAsync(AssetKey, typeof(GameObject));
            try
            {
                var locations = await locationsHandle.Task;
                if (locations.Count == 0)
                {
                    Debug.LogWarning($"Addressables has no locations for key '{AssetKey}'.");
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
                Debug.Log($"Addressables key '{AssetKey}': dependencies=[{dependencies}]");
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
#endif

            var gameObject = await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded || !gameObject)
            {
                var details = handle.OperationException?.ToString() ?? "No exception details were provided.";
                Addressables.Release(handle);
                _handle = null;
                throw new InvalidOperationException($"Addressables failed to load key '{AssetKey}'. {details}");
            }
            
            if (!gameObject.TryGetComponent<TComponent>(out _))
            {
                throw new NullReferenceException(
                    $"GameObject at path {AssetKey} does not contain component {typeof(TComponent)}.");
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
