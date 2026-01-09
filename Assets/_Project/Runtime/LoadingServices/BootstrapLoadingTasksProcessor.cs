using _Project.Runtime.Abstract.Services;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.LoadingServices
{
    public class BootstrapLoadingTasksProcessor : BaseLoadingTasksProcessor
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IRemoteConfigProvider _remoteConfigProvider;

        public BootstrapLoadingTasksProcessor(SceneLoader sceneLoader, IRemoteConfigProvider remoteConfigProvider)
            : base(sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _remoteConfigProvider = remoteConfigProvider;
        }

        protected override int SceneIndex => Constants.Scenes.Bootstrap;

        protected override async UniTask GetTasks()
        {
            var source = await _remoteConfigProvider.InitializeAsync();
            Debug.Log("Remote config loaded from source: " + source);
            await _sceneLoader.LoadSceneAsync(Constants.Scenes.Menu);
        }
    }
}