using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

namespace _Project.Runtime.Services
{
    public enum SaveServiceId
    {
        Local,
        Cloud
    }

    public interface ISaveService
    {
        UniTask<LoadResult<PlayerData>> TryLoad();
        UniTask<bool> Save(PlayerData data);
    }

    public sealed class UnitySaveService : ISaveService
    {
        public async UniTask<LoadResult<PlayerData>> TryLoad()
        {
            var key = AuthenticationService.Instance.PlayerId;
            if (string.IsNullOrWhiteSpace(key))
            {
                return LoadResult<PlayerData>.NotFound();
            }

            try
            {
                var keys = new HashSet<string> { key };
                var response = await CloudSaveService.Instance.Data.Player.LoadAsync(keys, new LoadOptions());
                if (response == null || !response.TryGetValue(key, out var item) || item?.Value == null)
                {
                    return LoadResult<PlayerData>.NotFound();
                }

                PlayerData parsed = null;
                try
                {
                    parsed = item.Value.GetAs<PlayerData>();
                }
                catch (Exception)
                {
                    var json = item.Value.GetAsString();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        parsed = JsonUtility.FromJson<PlayerData>(json);
                    }
                }

                return parsed == null
                    ? LoadResult<PlayerData>.NotFound()
                    : LoadResult<PlayerData>.Success(parsed);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[CloudSave] TryLoad failed for key '{key}'. {exception.Message}");
                return LoadResult<PlayerData>.NotFound();
            }
        }

        public async UniTask<bool> Save(PlayerData data)
        {
            var key = AuthenticationService.Instance.PlayerId;
            if (string.IsNullOrWhiteSpace(key) || data == null)
            {
                return false;
            }

            try
            {
                var payload = new Dictionary<string, object>
                {
                    { key, data }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(payload, new SaveOptions());
                Debug.Log("[CloudSave] Data saved to cloud");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[CloudSave] Save failed for key '{key}'. {exception.Message}");
                return false;
            }
        }
    }
}
