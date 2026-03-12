using System;
using Cysharp.Threading.Tasks;
using _Project.Runtime.Services;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace _Project.Runtime.Models
{
    public sealed class AuthenticationModel
    {
        public event Action AuthenticationCompleted;
        public event Action AuthenticationFailed;
        public event Action<SaveSelectionInfo> SaveSelectionRequired;

        private readonly PlayerDataSyncService _playerDataSyncService;
        private bool _authInProcess;
        private bool _selectionInProcess;
        private string _playerId;

        public AuthenticationModel(PlayerDataSyncService playerDataSyncService)
        {
            _playerDataSyncService = playerDataSyncService;
        }

        public void StartAuthentication()
        {
            if (_authInProcess)
            {
                return;
            }

            _authInProcess = true;
            UniTask.Void(SignUpAnonymouslyAsync);
        }

        public void SelectLocalSave()
        {
            StartSaveSelection(SaveSource.Local);
        }

        public void SelectCloudSave()
        {
            StartSaveSelection(SaveSource.Cloud);
        }

        private void EndAuthentication(bool succeeded)
        {
            _authInProcess = false;
            _selectionInProcess = false;
            if (succeeded)
            {
                AuthenticationCompleted?.Invoke();
            }
            else
            {
                AuthenticationFailed?.Invoke();
            }
        }
        
        private async UniTaskVoid SignUpAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded");
                
                _playerId = AuthenticationService.Instance.PlayerId;
                Debug.Log($"PlayerID: {_playerId}");

                var syncResult = await _playerDataSyncService.InitializeAsync(_playerId);
                
                switch (syncResult.Status)
                {
                    case PlayerDataSyncStatus.Completed:
                        EndAuthentication(succeeded: true);
                        return;
                    case PlayerDataSyncStatus.AwaitingSaveSelection:
                        if (!syncResult.SelectionInfo.HasValue)
                        {
                            Debug.LogWarning("[Auth] Save selection required but selection info is missing.");
                            EndAuthentication(succeeded: false);
                            return;
                        }

                        SaveSelectionRequired?.Invoke(syncResult.SelectionInfo.Value);
                        return;
                    case PlayerDataSyncStatus.Failed:
                    default:
                        Debug.LogWarning($"[Auth] Initial player data sync failed. {syncResult.ErrorMessage}");
                        EndAuthentication(succeeded: false);
                        return;
                }
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                EndAuthentication(succeeded: false);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                EndAuthentication(succeeded: false);
            }
        }

        private void StartSaveSelection(SaveSource source)
        {
            if (!_authInProcess || _selectionInProcess || string.IsNullOrWhiteSpace(_playerId))
            {
                return;
            }

            UniTask.Void(() => ResolveSaveSelectionAsync(source));
        }

        private async UniTaskVoid ResolveSaveSelectionAsync(SaveSource source)
        {
            _selectionInProcess = true;
            try
            {
                var result = await _playerDataSyncService.ResolveSelectionAsync(_playerId, source);
                if (result.Status == PlayerDataSyncStatus.Completed)
                {
                    EndAuthentication(succeeded: true);
                    return;
                }

                Debug.LogWarning($"[Auth] Resolving save selection failed. {result.ErrorMessage}");
                EndAuthentication(succeeded: false);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EndAuthentication(succeeded: false);
            }
        }
    }
}
