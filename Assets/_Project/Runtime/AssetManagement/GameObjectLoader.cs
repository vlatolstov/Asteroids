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
        Type Type { get; }
        UniTask<GameObject> LoadAsync();
        void Unload();
    }

    public class GameObjectLoader<TComponent> : IDisposable, IGameObjectLoader where TComponent : Component
    {
        public Type Type => typeof(TComponent);

        private readonly string _path;
        private readonly bool _createInstance;
        private GameObject _cachedGameObject;
        private AsyncOperationHandle<GameObject>? _handle;

        public GameObjectLoader(string assetPath, bool createInstance = false)
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

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var locationsHandle = Addressables.LoadResourceLocationsAsync(_path, typeof(GameObject));
            try
            {
                var locations = await locationsHandle.Task;
                if (locations.Count == 0)
                {
                    Debug.LogWarning($"Addressables has no locations for key '{_path}'.");
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
                Debug.Log($"Addressables key '{_path}': dependencies=[{dependencies}]");
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
                throw new InvalidOperationException($"Addressables failed to load key '{_path}'. {details}");
            }
            
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
