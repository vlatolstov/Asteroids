using UnityEngine.SceneManagement;

namespace _Project.Runtime.Constants
{
    public static class Scenes
    {
        public static int Bootstrap = SceneUtility.GetBuildIndexByScenePath("Bootstrap");
        public static int Loading = SceneUtility.GetBuildIndexByScenePath("Loading");
        public static int Menu = SceneUtility.GetBuildIndexByScenePath("Menu");
        public static int Game = SceneUtility.GetBuildIndexByScenePath("Game");
        public static int Empty = SceneUtility.GetBuildIndexByScenePath("Empty");
    }
}