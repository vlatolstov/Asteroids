using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Runtime.SceneManagement
{
    public class SceneLoader
    {
        private int _loadingSceneIndex;

        public async UniTask LoadSceneAsync(int sceneIndex)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex == sceneIndex)
            {
                Debug.Log($"You are trying to load active scene");
                return;
            }

            SceneManager.LoadScene(Constants.Scenes.Empty);

            _loadingSceneIndex = sceneIndex;

            await SceneManager.LoadSceneAsync(Constants.Scenes.Loading);
            await SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
            await UniTask.CompletedTask;
        }

        public async void Finish(int sceneIndex)
        {
            if (sceneIndex != _loadingSceneIndex)
            {
                Debug.LogError("You try to finish wrong scene");
                return;
            }

            await SceneManager.UnloadSceneAsync(Constants.Scenes.Loading);
        }
    }
}