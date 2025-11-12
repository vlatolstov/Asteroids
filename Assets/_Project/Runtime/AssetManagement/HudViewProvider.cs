using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class HudViewProvider : LocalAssetLoader<HudView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.HudView;
    }
}