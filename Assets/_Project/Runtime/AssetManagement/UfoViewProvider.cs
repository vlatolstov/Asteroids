using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Ufo;

namespace _Project.Runtime.AssetManagement
{
    public class UfoViewProvider : PrefabAssetLoader<UfoView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.UfoView;
    }
}
