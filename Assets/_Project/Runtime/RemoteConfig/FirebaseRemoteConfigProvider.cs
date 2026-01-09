using System;
using System.Collections.Generic;
using _Project.Runtime.Constants;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace _Project.Runtime.RemoteConfig
{
    public enum ConfigSource
    {
        Local,
        Remote
    }

    public sealed class FirebaseRemoteConfigProvider : IRemoteConfigProvider
    {
        private const string DefaultJsonResourcePath = "RemoteConfig/numeric_config_default";

        private readonly NumericConfigParser _parser;
        private Dictionary<string, object> _configMap = new();
        private bool _initialized;
        private ConfigSource _source;

        public FirebaseRemoteConfigProvider(NumericConfigParser parser)
        {
            _parser = parser;
        }

        public async UniTask<ConfigSource> InitializeAsync()
        {
            if (_initialized)
            {
                return _source;
            }

            _initialized = true;

            string defaultsJson = LoadDefaultsJson();
            var remote = FirebaseRemoteConfig.DefaultInstance;
            await remote.SetDefaultsAsync(new Dictionary<string, object>
            {
                { RemoteConfigKeys.NumericConfigJson, defaultsJson }
            });

            _configMap = _parser.Parse(defaultsJson);
            _source = ConfigSource.Local;
            
            await remote.FetchAsync(TimeSpan.Zero);
            await remote.ActivateAsync();
            string json = remote.GetValue(RemoteConfigKeys.NumericConfigJson).StringValue;

            if (!string.IsNullOrWhiteSpace(json))
            {
                var remoteMap = _parser.Parse(json);
                MergeConfigs(remoteMap);
                _source = ConfigSource.Remote;
            }

            return _source;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_configMap.TryGetValue(key, out var obj) && obj is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }

        private static string LoadDefaultsJson()
        {
            var asset = Resources.Load<TextAsset>(DefaultJsonResourcePath);
            return asset ? asset.text : string.Empty;
        }

        private void MergeConfigs(Dictionary<string, object> remoteMap)
        {
            foreach (var pair in remoteMap)
            {
                _configMap[pair.Key] = pair.Value;
            }
        }
    }
}