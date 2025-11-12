using _Project.Runtime.Abstract.AssetManagement;
using _Project.Runtime.Views;

namespace _Project.Runtime.AssetManagement
{
    public class ProjectileViewProvider : PrefabAssetLoader<ProjectileView>
    {
        protected override string AssetPath => Constants.AddressablesPrefabsPaths.ProjectileView;
    }
}
