using _Project.Runtime.Abstract.Ads;
using _Project.Runtime.Ads;
using _Project.Runtime.InAppPurchase;
using _Project.Runtime.Models;
using _Project.Runtime.RemoteConfig;
using _Project.Runtime.SceneManagement;
using _Project.Runtime.Services;
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
                .Bind<NumericConfigParser>()
                .AsSingle();

            Container
                .Bind(typeof(IRemoteConfigProvider), typeof(IInitializable))
                .To<FirebaseRemoteConfigProvider>()
                .AsSingle();

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

            Container.Bind(typeof(IAdsInitializer), typeof(IInitializable))
                .To<UnityAdsInitializer>()
                .AsSingle();

            Container
                .Bind<IAdsPlayer>()
                .To<UnityAdsPlayer>()
                .AsSingle();

            Container
                .Bind<ISaveService>()
                .To<LocalSaveService>()
                .AsSingle();

            Container
                .Bind<PlayerModel>()
                .AsSingle();

            Container
                .Bind<PlayerDataManager>()
                .AsSingle();

            Container.Bind(typeof(IIapService))
                .To<UnityIapService>()
                .AsSingle();
        }
    }
}
