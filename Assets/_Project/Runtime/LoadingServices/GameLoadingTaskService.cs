using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Pooling;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Services;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.LoadingServices
{
    public class GameLoadingTaskService : BaseLoadingTaskService
    {
        private readonly IViewPoolsService _poolsService;
        private readonly ViewsContainer _viewsContainer;
        private readonly HudViewProvider _hudViewProvider;
        private readonly BackgroundViewProvider _backgroundViewProvider;
        private readonly BGMViewProvider _bgmViewProvider;
        private readonly IConfigsService _configsService;

        public GameLoadingTaskService(SceneLoader sceneLoader, IViewPoolsService poolsService, ViewsContainer viewsContainer, HudViewProvider hudViewProvider, BackgroundViewProvider backgroundViewProvider, BGMViewProvider bgmViewProvider, IConfigsService configsService) : base(sceneLoader)
        {
            _poolsService = poolsService;
            _viewsContainer = viewsContainer;
            _hudViewProvider = hudViewProvider;
            _backgroundViewProvider = backgroundViewProvider;
            _bgmViewProvider = bgmViewProvider;
            _configsService = configsService;
        }

        protected override int SceneIndex => Constants.Scenes.Game;
        
        protected override async UniTask GetTasks()
        {
            await _configsService.LoadAllAsync();
            await _poolsService.LoadPoolsAsync();
            _viewsContainer.AddView(await _hudViewProvider.LoadAsync());
            _viewsContainer.AddView(await _backgroundViewProvider.LoadAsync());
            _viewsContainer.AddView(await _bgmViewProvider.LoadAsync());
            
            Debug.Log("Game loaded");
            await UniTask.NextFrame();
        }
    }
}
