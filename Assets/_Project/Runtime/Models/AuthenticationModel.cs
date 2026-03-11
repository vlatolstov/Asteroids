using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace _Project.Runtime.Models
{
    public sealed class AuthenticationModel
    {
        public event Action AuthenticationStarted;
        public event Action AuthenticationCompleted;
        public event Action AuthenticationFailed;
        private bool _authInProcess;

        public void StartAuthentication()
        {
            if (_authInProcess)
            {
                return;
            }
            
            AuthenticationStarted?.Invoke();
            UniTask.Void(SignUpAnonymouslyAsync);
        }

        private void CompleteAuthentication()
        {
            _authInProcess = false;
            AuthenticationCompleted?.Invoke();
        }

        private void FailAuthentication()
        {
            _authInProcess = false;
            AuthenticationFailed?.Invoke();
        }
        
        private async UniTaskVoid SignUpAnonymouslyAsync()
        {
            try
            {
                _authInProcess = true;
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
                CompleteAuthentication();
            }
            catch (AuthenticationException ex)
            {
                FailAuthentication();
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                FailAuthentication();
                Debug.LogException(ex);
            }
        }
    }
}
