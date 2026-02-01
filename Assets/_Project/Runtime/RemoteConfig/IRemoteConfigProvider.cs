using Cysharp.Threading.Tasks;

namespace _Project.Runtime.RemoteConfig
{
    public interface IRemoteConfigProvider
    {
        ConfigSource Source { get; }
        bool IsInitialized { get; }
        bool TryGet<T>(string key, out T value);
    }
    
    public enum ConfigSource
    {
        Local,
        Remote
    }
}
