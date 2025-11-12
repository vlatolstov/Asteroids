using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class BackgroundViewProvider : LocalAssetLoader<BackgroundView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.BackgroundView;
    }
}