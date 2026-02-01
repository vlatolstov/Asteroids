using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Constants;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.LoadingServices
{
    public class MenuLoadingTasksProcessor : BaseLoadingTasksProcessor
    {
        private readonly SceneAssetProvider _assetProvider;

        public MenuLoadingTasksProcessor(SceneLoader sceneLoader,
            SceneAssetProvider assetProvider) : base(sceneLoader)
        {
            _assetProvider = assetProvider;
        }

        protected override int SceneIndex => Constants.Scenes.Menu;

        protected override async UniTask GetTasks()
        {
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<MenuView>(AddressablesPrefabsPaths.MenuView, true));
            await _assetProvider.LoadAllAsync();
            Debug.Log("Menu loaded.");
        }
    }
}