using System;
using _Project.Runtime.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

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
        private readonly ISaveService _localSaveService;
        private readonly ISaveService _cloudSaveService;
        private readonly PlayerAutoSaveService _playerAutoSaveService;
        private string _pendingPlayerId;
        private PlayerData _pendingLocalData;
        private PlayerData _pendingCloudData;

        public PlayerDataSyncService(
            [Inject(Id = SaveServiceId.Local)] ISaveService localSaveService,
            [Inject(Id = SaveServiceId.Cloud)] ISaveService cloudSaveService,
            PlayerAutoSaveService playerAutoSaveService)
        {
            _localSaveService = localSaveService;
            _cloudSaveService = cloudSaveService;
            _playerAutoSaveService = playerAutoSaveService;
        }

        public async UniTask<PlayerDataSyncResult> InitializeAsync(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return PlayerDataSyncResult.Failed("PlayerId is empty.");
            }

            var localResult = await _localSaveService.TryLoad();
            if (!HasInternetConnection())
            {
                var selectedOfflineData = PickDataForOfflineFlow(localResult);
                _playerAutoSaveService.InitializeForPlayer(playerId, selectedOfflineData);
                ClearPendingSelection();
                return PlayerDataSyncResult.Completed();
            }

            var cloudResult = await _cloudSaveService.TryLoad();
            if (localResult.Found && cloudResult.Found && IsLocalNewer(localResult.Data, cloudResult.Data))
            {
                StorePendingSelection(playerId, localResult.Data, cloudResult.Data);
                return PlayerDataSyncResult.AwaitingSaveSelection(new SaveSelectionInfo(
                    localResult.Data.LastSavedAtUnixMs,
                    cloudResult.Data.LastSavedAtUnixMs));
            }

            var selectedOnlineData = PickDataForOnlineFlow(localResult, cloudResult);
            selectedOnlineData.Normalize();
            _playerAutoSaveService.InitializeForPlayer(playerId, selectedOnlineData);
            await _cloudSaveService.Save(selectedOnlineData);
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
                ? _pendingCloudData.Clone()
                : _pendingLocalData.Clone();
            ClearPendingSelection();

            selectedData.Normalize();
            _playerAutoSaveService.InitializeForPlayer(playerId, selectedData);
            await _cloudSaveService.Save(selectedData);
            return PlayerDataSyncResult.Completed();
        }

        private void StorePendingSelection(string playerId, PlayerData localData, PlayerData cloudData)
        {
            _pendingPlayerId = playerId;
            _pendingLocalData = localData.Clone();
            _pendingCloudData = cloudData.Clone();
        }

        private void ClearPendingSelection()
        {
            _pendingPlayerId = null;
            _pendingLocalData = null;
            _pendingCloudData = null;
        }

        private static PlayerData PickDataForOfflineFlow(LoadResult<PlayerData> localResult)
        {
            return localResult.Found ? localResult.Data : new PlayerData();
        }

        private static PlayerData PickDataForOnlineFlow(
            LoadResult<PlayerData> localResult,
            LoadResult<PlayerData> cloudResult)
        {
            return cloudResult.Found ? cloudResult.Data : PickDataForOfflineFlow(localResult);
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
    }
}
