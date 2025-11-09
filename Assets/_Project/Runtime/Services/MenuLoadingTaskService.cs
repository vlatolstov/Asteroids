using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;

namespace _Project.Runtime.Services
{
    public class MenuLoadingTaskService : BaseLoadingTaskService
    {
        private readonly MenuViewLoader _menuViewLoader;
        public MenuLoadingTaskService(SceneLoader sceneLoader, MenuViewLoader menuViewLoader) : base(sceneLoader)
        {
            _menuViewLoader = menuViewLoader;
        }

        protected override int SceneIndex => Constants.Scenes.Menu;
        protected override async UniTask GetTasks()
        {
            await _menuViewLoader.Load();
        }
    }
}