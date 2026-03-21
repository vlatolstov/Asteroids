using _Project.Runtime.Abstract.Services;
using _Project.Runtime.AssetManagement;
using _Project.Runtime.Asteroid;
using _Project.Runtime.Constants;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Services;
using _Project.Runtime.Ship;
using _Project.Runtime.Ufo;
using _Project.Runtime.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.LoadingServices
{
    public class GameLoadingTasksProcessor : BaseLoadingTasksProcessor
    {
        private readonly SceneAssetProvider _assetProvider;
        private readonly IResourcesService _resourcesService;

        public GameLoadingTasksProcessor(SceneLoader sceneLoader,
            IResourcesService resourcesService, SceneAssetProvider assetProvider) : base(sceneLoader)
        {
            _resourcesService = resourcesService;
            _assetProvider = assetProvider;
        }

        protected override int SceneIndex => Constants.Scenes.Game;

        protected override async UniTask GetTasks()
        {
            await _resourcesService.LoadAllAsync();
            
            _assetProvider.RegisterLoader(new GameObjectLoader<HudView>(AddressablesPrefabsPaths.HudView, true));
            _assetProvider.RegisterLoader(
                new GameObjectLoader<BackgroundView>(AddressablesPrefabsPaths.BackgroundView, true));
            _assetProvider.RegisterLoader(new GameObjectLoader<BGMView>(AddressablesPrefabsPaths.BGMView, true));

            _assetProvider.RegisterLoader(new GameObjectLoader<ShipView>(AddressablesPrefabsPaths.ShipView));
            _assetProvider.RegisterLoader(new GameObjectLoader<UfoView>(AddressablesPrefabsPaths.UfoView));
            _assetProvider.RegisterLoader(new GameObjectLoader<AsteroidView>(AddressablesPrefabsPaths.AsteroidView));
            _assetProvider.RegisterLoader(new GameObjectLoader<ProjectileView>(AddressablesPrefabsPaths.ProjectileView));
            _assetProvider.RegisterLoader(new GameObjectLoader<AoeAttackView>(AddressablesPrefabsPaths.AoeAttackView));
            _assetProvider.RegisterLoader(new GameObjectLoader<AudioSourceView>(AddressablesPrefabsPaths.AudioSourceView));
            _assetProvider.RegisterLoader(new GameObjectLoader<AnimationView>(AddressablesPrefabsPaths.AnimationView));
            
            await _assetProvider.LoadAllAsync();
            
            Debug.Log("Game loaded");
        }
    }
}