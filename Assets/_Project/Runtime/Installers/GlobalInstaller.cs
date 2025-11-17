using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.Ads;
using _Project.Runtime.SceneManagement;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "GlobalInstaller", menuName = "Installers/Global Installer")]
    public class GlobalInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private AndroidAdsSettings _androidAdsSettings;

        [SerializeField]
        private IOsUnityAdsSettings _iOsAdsSettings;

        public override void InstallBindings()
        {
            Container
                .Bind<SceneLoader>()
                .AsSingle();

            Container
                .Bind<IAdsSettings>()
#if UNITY_IOS
                .FromInstance(_iOsAdsSettings)
#else
                .FromInstance(_androidAdsSettings)
#endif
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<UnityAdsInitializer>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<UnityAdsPlayer>()
                .AsSingle();
        }
    }
}