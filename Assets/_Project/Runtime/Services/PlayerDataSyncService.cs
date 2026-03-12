using System;
using System.Collections.Generic;
using _Project.Runtime.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Runtime.Services
{
    public enum PlayerDataSyncStatus
    {
        Completed,
        AwaitingSaveSelection,
        Failed
    }

    public enum SaveSource
    {
        Local,
        Cloud
    }

    public readonly struct SaveSelectionInfo
    {
        public long LocalLastSavedAtUnixMs { get; }
        public long CloudLastSavedAtUnixMs { get; }

        public SaveSelectionInfo(long localLastSavedAtUnixMs, long cloudLastSavedAtUnixMs)
        {
            LocalLastSavedAtUnixMs = Math.Max(0, localLastSavedAtUnixMs);
            CloudLastSavedAtUnixMs = Math.Max(0, cloudLastSavedAtUnixMs);
        }
    }

    public readonly struct PlayerDataSyncResult
    {
        public PlayerDataSyncStatus Status { get; }
        public string ErrorMessage { get; }
        public SaveSelectionInfo? SelectionInfo { get; }

        private PlayerDataSyncResult(PlayerDataSyncStatus status, string errorMessage, SaveSelectionInfo? selectionInfo)
        {
            Status = status;
            ErrorMessage = errorMessage;
            SelectionInfo = selectionInfo;
        }

        public static PlayerDataSyncResult Completed()
        {
            return new PlayerDataSyncResult(PlayerDataSyncStatus.Completed, null, null);
        }

        public static PlayerDataSyncResult AwaitingSaveSelection(SaveSelectionInfo selectionInfo)
        {
            return new PlayerDataSyncResult(PlayerDataSyncStatus.AwaitingSaveSelection, null, selectionInfo);
        }

        public static PlayerDataSyncResult Failed(string errorMessage)
        {
            return new PlayerDataSyncResult(PlayerDataSyncStatus.Failed, errorMessage, null);
        }
    }

    public sealed class PlayerDataSyncService
    {
        private readonly ILocalSaveService _localSaveService;
        private readonly ICloudSaveService _cloudSaveService;
        private readonly PlayerDataManager _playerDataManager;
        private string _pendingPlayerId;
        private PlayerData _pendingLocalData;
        private PlayerData _pendingCloudData;

        public PlayerDataSyncService(ILocalSaveService localSaveService, ICloudSaveService cloudSaveService,
            PlayerDataManager playerDataManager)
        {
            _localSaveService = localSaveService;
            _cloudSaveService = cloudSaveService;
            _playerDataManager = playerDataManager;
        }

        public async UniTask<PlayerDataSyncResult> InitializeAsync(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return PlayerDataSyncResult.Failed("PlayerId is empty.");
            }

            var localResult = TryLoadLocal(playerId);
            if (!HasInternetConnection())
            {
                var selectedOfflineData = PickDataForOfflineFlow(localResult);
                _playerDataManager.InitializeForPlayer(playerId, selectedOfflineData);
                ClearPendingSelection();
                return PlayerDataSyncResult.Completed();
            }

            var cloudResult = await _cloudSaveService.TryLoad(playerId);
            if (localResult.Found && cloudResult.Found && IsLocalNewer(localResult.Data, cloudResult.Data))
            {
                StorePendingSelection(playerId, localResult.Data, cloudResult.Data);
                return PlayerDataSyncResult.AwaitingSaveSelection(new SaveSelectionInfo(
                    localResult.Data.LastSavedAtUnixMs,
                    cloudResult.Data.LastSavedAtUnixMs));
            }

            var selectedOnlineData = PickDataForOnlineFlow(localResult, cloudResult);
            _playerDataManager.InitializeForPlayer(playerId, selectedOnlineData);
            await _cloudSaveService.Save(playerId, selectedOnlineData);
            ClearPendingSelection();
            return PlayerDataSyncResult.Completed();
        }

        public async UniTask<PlayerDataSyncResult> ResolveSelectionAsync(string playerId, SaveSource source)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return PlayerDataSyncResult.Failed("PlayerId is empty.");
            }

            if (string.IsNullOrWhiteSpace(_pendingPlayerId) || _pendingLocalData == null || _pendingCloudData == null)
            {
                return PlayerDataSyncResult.Failed("No pending save selection.");
            }

            if (!string.Equals(_pendingPlayerId, playerId, StringComparison.Ordinal))
            {
                return PlayerDataSyncResult.Failed("Pending save selection belongs to another player.");
            }

            var selectedData = source == SaveSource.Cloud
                ? Clone(_pendingCloudData)
                : Clone(_pendingLocalData);
            ClearPendingSelection();

            _playerDataManager.InitializeForPlayer(playerId, selectedData);
            await _cloudSaveService.Save(playerId, selectedData);
            return PlayerDataSyncResult.Completed();
        }

        private LoadResult<PlayerData> TryLoadLocal(string playerId)
        {
            return _localSaveService.TryLoad<PlayerData>(playerId);
        }

        private void StorePendingSelection(string playerId, PlayerData localData, PlayerData cloudData)
        {
            _pendingPlayerId = playerId;
            _pendingLocalData = Clone(localData);
            _pendingCloudData = Clone(cloudData);
        }

        private void ClearPendingSelection()
        {
            _pendingPlayerId = null;
            _pendingLocalData = null;
            _pendingCloudData = null;
        }

        private static PlayerData PickDataForOfflineFlow(LoadResult<PlayerData> localResult)
        {
            return localResult.Found ? localResult.Data : CreateEmptyPlayerData();
        }

        private static PlayerData PickDataForOnlineFlow(
            LoadResult<PlayerData> localResult,
            LoadResult<PlayerData> cloudResult)
        {
            if (cloudResult.Found)
            {
                return cloudResult.Data;
            }

            if (localResult.Found)
            {
                return localResult.Data;
            }

            return CreateEmptyPlayerData();
        }

        private static bool IsLocalNewer(PlayerData localData, PlayerData cloudData)
        {
            var localTimestamp = Math.Max(0, localData?.LastSavedAtUnixMs ?? 0);
            var cloudTimestamp = Math.Max(0, cloudData?.LastSavedAtUnixMs ?? 0);
            return localTimestamp > cloudTimestamp;
        }

        private static bool HasInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        private static PlayerData CreateEmptyPlayerData()
        {
            return new PlayerData();
        }

        private static PlayerData Clone(PlayerData source)
        {
            source ??= new PlayerData();

            var clone = new PlayerData
            {
                LastSavedAtUnixMs = Math.Max(0, source.LastSavedAtUnixMs),
                BestScore = Math.Max(0, source.BestScore),
                NonConsumableProductIds = new List<string>(source.NonConsumableProductIds ?? new List<string>()),
                ActiveSubscriptionProductIds = new List<string>(source.ActiveSubscriptionProductIds ?? new List<string>()),
                Consumables = new List<ConsumableData>()
            };

            var consumables = source.Consumables ?? new List<ConsumableData>();
            for (var i = 0; i < consumables.Count; i++)
            {
                var item = consumables[i];
                if (item == null || string.IsNullOrWhiteSpace(item.ProductId) || item.Amount <= 0)
                {
                    continue;
                }

                clone.Consumables.Add(new ConsumableData
                {
                    ProductId = item.ProductId,
                    Amount = item.Amount
                });
            }

            return clone;
        }
    }
}
