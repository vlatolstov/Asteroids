using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Runtime.Abstract.Services
{
    public abstract class BaseLoadingTaskService : IInitializable, ITickable
    {
        private SceneLoader _sceneLoader;

        private UniTask _pendingTask;
        private bool _inProgress;

        protected abstract int SceneIndex { get; }

        protected BaseLoadingTaskService(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            _pendingTask = GetTasks();
            _inProgress = true;
        }

        public void Tick()
        {
            if (!_inProgress)
            {
                return;
            }

            if (_pendingTask.Status != UniTaskStatus.Pending)
            {
                _inProgress = false;
            }

            if (_pendingTask.Status == UniTaskStatus.Succeeded)
            {
                _sceneLoader.Finish(SceneIndex);
            }
        }

        protected abstract UniTask GetTasks();
    }
}