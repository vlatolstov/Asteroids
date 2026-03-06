using System;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public interface ISaveService
    {
        bool TryLoad<T>(string key, out T data) where T : class;
        bool Save<T>(string key, T data) where T : class;
        bool Delete(string key);
        void ClearAll();
    }

    public sealed class LocalSaveService : ISaveService
    {
        public bool TryLoad<T>(string key, out T data) where T : class
        {
            data = null;

            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            var json = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {
                data = JsonUtility.FromJson<T>(json);
                return data != null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Save] Failed to load key '{key}'. {exception.Message}");
                return false;
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

        public bool Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (!PlayerPrefs.HasKey(key))
            {
                return false;
            }

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            return true;
        }

        public void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
