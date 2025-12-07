using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.LoadingServices
{
    public class MenuLoadingTasksProcessor : BaseLoadingTasksProcessor
    {
        private readonly LocalAssetProvider _assetProvider;

        public MenuLoadingTasksProcessor(SceneLoader sceneLoader,
            LocalAssetProvider assetProvider) : base(sceneLoader)
        {
            _assetProvider = assetProvider;
        }

        protected override int SceneIndex => Constants.Scenes.Menu;

        protected override async UniTask GetTasks()
        {
            await _assetProvider.LoadAsync(Constants.AddressablesPrefabsPaths.MenuView);
            Debug.Log("Menu loaded.");
        }
    }
}