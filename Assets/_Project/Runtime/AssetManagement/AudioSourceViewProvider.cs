using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class AudioSourceViewProvider : PrefabAssetLoader<AudioSourceView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.AudioSourceView;
    }
}
