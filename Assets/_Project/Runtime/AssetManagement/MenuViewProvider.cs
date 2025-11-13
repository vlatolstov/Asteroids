using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class MenuViewProvider : LocalAssetLoader<MenuView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.MenuView;
    }
}