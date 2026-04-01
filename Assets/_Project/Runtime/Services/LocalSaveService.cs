using System;
using _Project.Runtime.Constants;
using _Project.Runtime.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public sealed class LocalSaveService : ISaveService
    {
        private const string Key = DataKeys.LocalPlayerDataKey;
        
        public UniTask<LoadResult<PlayerData>> TryLoad()
        {
            
            if (string.IsNullOrWhiteSpace(Key))
            {
                return UniTask.FromResult(LoadResult<PlayerData>.NotFound());
            }

            var json = PlayerPrefs.GetString(Key, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return UniTask.FromResult(LoadResult<PlayerData>.NotFound());
            }

            try
            {
                var data = JsonUtility.FromJson<PlayerData>(json);
                var result = data == null
                    ? LoadResult<PlayerData>.NotFound()
                    : LoadResult<PlayerData>.Success(data);
                return UniTask.FromResult(result);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Save] Failed to load key '{Key}'. {exception.Message}");
                return UniTask.FromResult(LoadResult<PlayerData>.NotFound());
            }
        }

        public UniTask<bool> Save(PlayerData data)
        {
            if (string.IsNullOrWhiteSpace(Key) || data == null)
            {
                return UniTask.FromResult(false);
            }

            try
            {
                var json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(Key, json);
                PlayerPrefs.Save();
                return UniTask.FromResult(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Save] Failed to save key '{Key}'. {exception.Message}");
                return UniTask.FromResult(false);
            }
        }
    }
}
