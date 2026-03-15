using System;
using System.IO;
using _Project.Runtime.Abstract.Services;
using _Project.Runtime.InAppPurchase;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Runtime.LoadingServices
{
    public class BootstrapLoadingTasksProcessor : BaseLoadingTasksProcessor
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IIapService _unityIapService;

        public BootstrapLoadingTasksProcessor(SceneLoader sceneLoader,
            IIapService unityIapService)
            : base(sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _unityIapService = unityIapService;
        }

        protected override int SceneIndex => Constants.Scenes.Bootstrap;

        protected override async UniTask GetTasks()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!Caching.ClearCache())
            {
                Debug.LogWarning("Unity cache was not fully cleared.");
            }

            var path = Path.Combine(Application.persistentDataPath, "com.unity.addressables");
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to delete Addressables cache folder: {e.Message}");
            }
#endif

            await Addressables.InitializeAsync();
            
            await UnityServices.InitializeAsync();
            
            await _unityIapService.Connect();
            _unityIapService.FetchProducts();
            
            await _sceneLoader.LoadSceneAsync(Constants.Scenes.Authentication);
        }
    }
}