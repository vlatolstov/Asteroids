using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Ship;

namespace _Project.Runtime.AssetManagement
{
    public class ShipViewProvider : PrefabAssetLoader<ShipView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.ShipView;
    }
}
