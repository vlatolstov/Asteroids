using _Project.Runtime.Abstract.Services;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;

namespace _Project.Runtime.LoadingServices
{
    public class BootstrapLoadingTaskService : BaseLoadingTaskService
    {
        private SceneLoader _sceneLoader;

        public BootstrapLoadingTaskService(SceneLoader sceneLoader) : base(sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        protected override int SceneIndex => Constants.Scenes.Bootstrap;
        
        protected override async UniTask GetTasks()
        {
            await _sceneLoader.LoadSceneAsync(Constants.Scenes.Menu);
        }
    }
}