using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Asteroid;

namespace _Project.Runtime.AssetManagement
{
    public class AsteroidViewProvider : PrefabAssetLoader<AsteroidView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.AsteroidView;
    }
}
