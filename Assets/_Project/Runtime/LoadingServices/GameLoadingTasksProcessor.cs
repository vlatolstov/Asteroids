using _Project.Runtime.Abstract.AssetManagement;
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
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<HudView>(AddressablesPrefabsPaths.HudView, true));
            _assetProvider.RegisterLoader(
                new LocalGameObjectLoader<BackgroundView>(AddressablesPrefabsPaths.BackgroundView, true));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<BGMView>(AddressablesPrefabsPaths.BGMView, true));

            _assetProvider.RegisterLoader(new LocalGameObjectLoader<ShipView>(AddressablesPrefabsPaths.ShipView));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<UfoView>(AddressablesPrefabsPaths.UfoView));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<AsteroidView>(AddressablesPrefabsPaths.AsteroidView));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<ProjectileView>(AddressablesPrefabsPaths.ProjectileView));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<AoeAttackView>(AddressablesPrefabsPaths.AoeAttackView));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<AudioSourceView>(AddressablesPrefabsPaths.AudioSourceView));
            _assetProvider.RegisterLoader(new LocalGameObjectLoader<AnimationView>(AddressablesPrefabsPaths.AnimationView));
            
            await _assetProvider.LoadAllAsync();

            await _resourcesService.LoadAllAsync();

            Debug.Log("Game loaded");
            await UniTask.NextFrame();
        }
    }
}