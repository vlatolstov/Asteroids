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
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField]
        private AndroidAdsSettings androidAdsSettings;

        [SerializeField]
        private IOsUnityAdsSettings iOsAdsSettings;

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
                .FromInstance(iOsAdsSettings)
#else
                .FromInstance(androidAdsSettings)
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
                .Bind<LocalSaveService>()
                .AsSingle();

            Container
                .Bind<UnitySaveService>()
                .AsSingle();

            Container
                .Bind<ISaveService>()
                .WithId(SaveServiceId.Local)
                .To<LocalSaveService>()
                .FromResolve();

            Container
                .Bind<ISaveService>()
                .WithId(SaveServiceId.Cloud)
                .To<UnitySaveService>()
                .FromResolve();

            Container
                .Bind<PlayerModel>()
                .AsSingle();

            Container
                .Bind<PlayerAutoSaveService>()
                .AsSingle();

            Container
                .Bind<PlayerDataSyncService>()
                .AsSingle();

            Container.Bind(typeof(IIapService))
                .To<UnityIapService>()
                .AsSingle();
        }
    }
}
