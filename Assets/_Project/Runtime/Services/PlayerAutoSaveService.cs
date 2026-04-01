using System;
using _Project.Runtime.Data;
using _Project.Runtime.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Services
{
    public sealed class PlayerAutoSaveService
    {
        private readonly ISaveService _localSaveService;
        private readonly ISaveService _cloudSaveService;
        private readonly PlayerModel _playerModel;

        private string _playerId;
        private bool _missingPlayerIdLogged;
        private long _lastSavedAtUnixMs;

        public PlayerAutoSaveService(
            [Inject(Id = SaveServiceId.Local)] ISaveService localSaveService,
            [Inject(Id = SaveServiceId.Cloud)] ISaveService cloudSaveService,
            PlayerModel playerModel)
        {
            _localSaveService = localSaveService;
            _cloudSaveService = cloudSaveService;
            _playerModel = playerModel;
            _playerModel.StateChanged += OnPlayerStateChanged;
        }

        private bool IsInitialized => !string.IsNullOrWhiteSpace(_playerId);

        public void InitializeForPlayer(string playerId, PlayerData initialData)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("PlayerId is null or empty.", nameof(playerId));
            }

            _playerId = playerId;
            _missingPlayerIdLogged = false;

            initialData ??= new PlayerData();
            initialData.Normalize();
            
            _lastSavedAtUnixMs = initialData.LastSavedAtUnixMs;
            _playerModel.LoadSnapshot(initialData);

            UniTask.Void(() => SaveLocalSilentlyAsync(CreateSnapshotForPersistence(advanceTimestamp: false)));
        }

        private void OnPlayerStateChanged()
        {
            if (!IsInitialized)
            {
                if (_missingPlayerIdLogged)
                {
                    return;
                }

                Debug.LogWarning("[PlayerData] PlayerId is not initialized yet, data changes are in-memory only.");
                _missingPlayerIdLogged = true;
                return;
            }

            var snapshot = CreateSnapshotForPersistence(advanceTimestamp: true);
            UniTask.Void(() => SaveLocalSilentlyAsync(snapshot));
            UniTask.Void(() => SaveCloudSilentlyAsync(snapshot));
        }

        private async UniTaskVoid SaveLocalSilentlyAsync(PlayerData data)
        {
            if (data == null)
            {
                return;
            }

            if (!await _localSaveService.Save(data.Clone()))
            {
                Debug.LogWarning("[PlayerData] Failed to persist player data locally.");
            }
        }

        private async UniTaskVoid SaveCloudSilentlyAsync(PlayerData data)
        {
            if (data == null)
            {
                return;
            }

            await _cloudSaveService.Save(data.Clone());
        }

        private PlayerData CreateSnapshotForPersistence(bool advanceTimestamp)
        {
            var snapshot = _playerModel.CreateSnapshot();
            if (advanceTimestamp)
            {
                _lastSavedAtUnixMs = NextTimestamp(_lastSavedAtUnixMs);
            }

            snapshot.LastSavedAtUnixMs = Math.Max(0, _lastSavedAtUnixMs);
            return snapshot;
        }
        
        private static long NextTimestamp(long previousTimestamp)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return now > previousTimestamp ? now : previousTimestamp + 1;
        }
    }
}
