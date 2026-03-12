using System;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public interface ILocalSaveService
    {
        LoadResult<T> TryLoad<T>(string key) where T : class;
        bool Save<T>(string key, T data) where T : class;
        void Delete(string key);
    }

    public sealed class LocalSaveService : ILocalSaveService
    {
        public LoadResult<T> TryLoad<T>(string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return LoadResult<T>.NotFound();
            }

            var json = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return LoadResult<T>.NotFound();
            }

            try
            {
                var data = JsonUtility.FromJson<T>(json);
                return data == null
                    ? LoadResult<T>.NotFound()
                    : LoadResult<T>.Success(data);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Save] Failed to load key '{key}'. {exception.Message}");
                return LoadResult<T>.NotFound();
            }
        }

        public bool Save<T>(string key, T data) where T : class
        {
            if (string.IsNullOrWhiteSpace(key) || data == null)
            {
                return false;
            }

            try
            {
                var json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Save] Failed to save key '{key}'. {exception.Message}");
                return false;
            }
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}
