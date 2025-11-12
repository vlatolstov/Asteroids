using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class AnimationViewProvider : PrefabAssetLoader<AnimationView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.AnimationView;
    }
}
