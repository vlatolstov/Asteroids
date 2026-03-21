using System;
using _Project.Runtime.Data;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public sealed class LocalSaveService : ISaveService
    {
        public UniTask<LoadResult<PlayerData>> TryLoad()
        {
            var key = AuthenticationService.Instance.PlayerId;
            if (string.IsNullOrWhiteSpace(key))
            {
                return UniTask.FromResult(LoadResult<PlayerData>.NotFound());
            }

            var json = PlayerPrefs.GetString(key, string.Empty);
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
                Debug.LogWarning($"[Save] Failed to load key '{key}'. {exception.Message}");
                return UniTask.FromResult(LoadResult<PlayerData>.NotFound());
            }
        }

        public UniTask<bool> Save(PlayerData data)
        {
            var key = AuthenticationService.Instance.PlayerId;
            if (string.IsNullOrWhiteSpace(key) || data == null)
            {
                return UniTask.FromResult(false);
            }

            try
            {
                var json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                return UniTask.FromResult(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Save] Failed to save key '{key}'. {exception.Message}");
                return UniTask.FromResult(false);
            }
        }
    }
}
