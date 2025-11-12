using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.LoadingServices
{
    public class MenuLoadingTaskService : BaseLoadingTaskService
    {
        private readonly ViewsContainer _viewsContainer;
        private readonly MenuViewProvider _menuViewProvider;
        public MenuLoadingTaskService(SceneLoader sceneLoader, 
            MenuViewProvider menuViewProvider,
            ViewsContainer viewsContainer) : base(sceneLoader)
        {
            _menuViewProvider = menuViewProvider;
            _viewsContainer = viewsContainer;
        }

        protected override int SceneIndex => Constants.Scenes.Menu;

        protected override async UniTask GetTasks()
        {
            var view = await _menuViewProvider.LoadAsync();
            _viewsContainer.AddView(view);
            Debug.Log("Menu loaded.");
        }
    }
}