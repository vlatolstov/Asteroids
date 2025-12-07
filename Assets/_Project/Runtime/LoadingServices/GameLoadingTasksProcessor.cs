using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.LoadingServices
{
    public class GameLoadingTasksProcessor : BaseLoadingTasksProcessor
    {
        private readonly IViewPoolsService _poolsService;
        private readonly LocalAssetProvider _assetProvider;
        private readonly IConfigsService _configsService;

        public GameLoadingTasksProcessor(SceneLoader sceneLoader, IViewPoolsService poolsService,
            IConfigsService configsService, LocalAssetProvider assetProvider) : base(sceneLoader)
        {
            _poolsService = poolsService;
            _configsService = configsService;
            _assetProvider = assetProvider;
        }

        protected override int SceneIndex => Constants.Scenes.Game;

        protected override async UniTask GetTasks()
        {
            await _configsService.LoadAllAsync();
            await _poolsService.LoadPoolsAsync();
            await _assetProvider.LoadAsync(Constants.AddressablesPrefabsPaths.HudView);
            await _assetProvider.LoadAsync(Constants.AddressablesPrefabsPaths.BackgroundView);
            await _assetProvider.LoadAsync(Constants.AddressablesPrefabsPaths.BGMView);

            Debug.Log("Game loaded");
            await UniTask.NextFrame();
        }
    }
}