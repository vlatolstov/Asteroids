using _Project.Runtime.Abstract.Services;
using _Project.Runtime.InAppPurchase;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;

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
            await _unityIapService.Connect();
            _unityIapService.FetchProducts();
            await _sceneLoader.LoadSceneAsync(Constants.Scenes.Menu);
        }
    }
}