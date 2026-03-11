namespace _Project.Runtime.RemoteConfig
{
    public interface IRemoteConfigProvider
    {
        bool TryGet<T>(string key, out T value);
    }
    
    public enum ConfigSource
    {
        Local,
        Remote
    }
}
