using _Project.Runtime.Abstract.Services;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;

namespace _Project.Runtime.Services
{
    public class GameLoadingTaskService : BaseLoadingTaskService
    {
        public GameLoadingTaskService(SceneLoader sceneLoader) : base(sceneLoader)
        { }

        protected override int SceneIndex => Constants.Scenes.Game;
        protected override async UniTask GetTasks()
        {
            await UniTask.CompletedTask;
        }
    }
}