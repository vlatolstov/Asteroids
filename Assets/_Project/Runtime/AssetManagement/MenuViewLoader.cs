using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;

namespace _Project.Runtime.AssetManagement
{
    public class MenuViewLoader : LocalAssetLoader
    {
        private const string Path = "MenuView";

        public async UniTask<MenuView> Load()
        {
            return await LoadAsyncInternal<MenuView>(Constants.AddressablesPaths.UI + Path);
        }

        public void Unload()
        {
            UnloadInternal();
        }
    }
}