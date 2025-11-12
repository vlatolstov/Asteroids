using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class BGMViewProvider : LocalAssetLoader<BGMView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.BGMView;
    }
}