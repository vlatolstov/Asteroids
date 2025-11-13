using System;
using _Project.Runtime.SceneManagement;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Runtime.Abstract.Services
{
    public abstract class BaseLoadingTaskService : IInitializable, ITickable
    {
        private readonly SceneLoader _sceneLoader;

        private UniTask _pendingTask;
        private bool _inProgress;
        
        public event Action OnTasksFinished;

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
                OnTasksFinished?.Invoke();
            }
        }

        protected abstract UniTask GetTasks();
    }
}