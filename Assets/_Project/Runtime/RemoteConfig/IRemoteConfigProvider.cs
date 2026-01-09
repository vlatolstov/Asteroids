using Cysharp.Threading.Tasks;

namespace _Project.Runtime.RemoteConfig
{
    public interface IRemoteConfigProvider
    {
        UniTask<ConfigSource> InitializeAsync();
        bool TryGet<T>(string key, out T value);
    }
}
