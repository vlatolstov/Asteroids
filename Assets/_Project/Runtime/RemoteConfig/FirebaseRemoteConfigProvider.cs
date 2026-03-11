using System;
using System.Collections.Generic;
using _Project.Runtime.Constants;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.RemoteConfig;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.RemoteConfig
{
    public sealed class FirebaseRemoteConfigProvider : IInitializable, IRemoteConfigProvider
    {
        private const string DefaultJsonResourcePath = "RemoteConfig/numeric_config_default";

        private readonly NumericConfigParser _parser;
        private Dictionary<string, object> _configMap = new();

        public FirebaseRemoteConfigProvider(NumericConfigParser parser)
        {
            _parser = parser;
        }

        public async void Initialize()
        {
            try
            {
                var status = await FirebaseApp.CheckAndFixDependenciesAsync();

                if (status != DependencyStatus.Available)
                {
                    Debug.LogError($"Firebase init failed: {status}");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                var source = await InitializeAsyncInternal();
                Debug.Log("Numeric configs loaded from source: " + source);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask<ConfigSource> InitializeAsyncInternal()
        {
            string defaultsJson = LoadDefaultsJson();
            var remote = FirebaseRemoteConfig.DefaultInstance;
            await remote.SetDefaultsAsync(new Dictionary<string, object>
            {
                { RemoteConfigKeys.NumericConfigJson, defaultsJson }
            });

            _configMap = _parser.Parse(defaultsJson);
            var source = ConfigSource.Local;

            await remote.FetchAsync(TimeSpan.Zero);
            await remote.ActivateAsync();
            string json = remote.GetValue(RemoteConfigKeys.NumericConfigJson).StringValue;

            if (!string.IsNullOrWhiteSpace(json))
            {
                var remoteMap = _parser.Parse(json);
                MergeConfigs(remoteMap);
                source = ConfigSource.Remote;
            }

            return source;
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