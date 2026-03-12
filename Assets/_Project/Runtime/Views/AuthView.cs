using System;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Services;
using _Project.Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class AuthView : BaseView
    {
        private UIDocument _doc;
        private VisualElement _root;
        private VisualElement _selectSaveWindow;
        private Label _saveSelectionInfoField;
        private Button _signInButton;
        private Button _selectLocalButton;
        private Button _selectCloudButton;
        
        private const string SingInText = "Sign In";
        private const string LoadingText = "Loading...";

        public event Action SignInClicked;
        public event Action LocalSaveSelected;
        public event Action CloudSaveSelected;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
            _root = _doc.rootVisualElement;
            if (_root == null)
            {
                Debug.LogError("[AuthView] UIDocument rootVisualElement was null.");
                return;
            }

            _signInButton = _root.Q<Button>("sign-in-btn");
            _selectSaveWindow = _root.Q<VisualElement>("select-save-window");
            _saveSelectionInfoField = _root.Q<Label>("select-save-info-field");
            _selectLocalButton = _root.Q<Button>("select-save-local-btn");
            _selectCloudButton = _root.Q<Button>("select-save-cloud-btn");

            if (_signInButton != null)
            {
                _signInButton.clicked += OnSignInButtonClicked;
            }

            if (_selectLocalButton != null)
            {
                _selectLocalButton.clicked += OnSelectLocalButtonClicked;
            }

            if (_selectCloudButton != null)
            {
                _selectCloudButton.clicked += OnSelectCloudButtonClicked;
            }

            HideSaveSelectionWindow();
        }

        private void OnDestroy()
        {
            if (_signInButton != null)
            {
                _signInButton.clicked -= OnSignInButtonClicked;
            }

            if (_selectLocalButton != null)
            {
                _selectLocalButton.clicked -= OnSelectLocalButtonClicked;
            }

            if (_selectCloudButton != null)
            {
                _selectCloudButton.clicked -= OnSelectCloudButtonClicked;
            }
        }

        public void ShowSaveSelectionWindow()
        {
            SetSaveSelectionWindowVisible(true);
        }

        public void HideSaveSelectionWindow()
        {
            SetSaveSelectionWindowVisible(false);
            SetSaveSelectionInfoText(string.Empty);
        }

        public void SetSaveSelectionInfo(SaveSelectionInfo selectionInfo)
        {
            var localText = TimeHelpers.FormatTimestamp(selectionInfo.LocalLastSavedAtUnixMs);
            var cloudText = TimeHelpers.FormatTimestamp(selectionInfo.CloudLastSavedAtUnixMs);
            SetSaveSelectionInfoText($"Local: {localText}\nCloud: {cloudText}");
        }

        private void SetSaveSelectionWindowVisible(bool visible)
        {
            if (_selectSaveWindow == null)
            {
                return;
            }

            _selectSaveWindow.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnSignInButtonClicked()
        {
            SignInClicked?.Invoke();
            _signInButton.text = LoadingText;
            _signInButton.SetEnabled(false);
        }

        public void RestoreSignInButton()
        {
            if (_signInButton == null)
            {
                return;
            }

            _signInButton.text = SingInText;
            _signInButton.SetEnabled(true);
        }

        private void OnSelectLocalButtonClicked()
        {
            LocalSaveSelected?.Invoke();
        }

        private void OnSelectCloudButtonClicked()
        {
            CloudSaveSelected?.Invoke();
        }

        private void SetSaveSelectionInfoText(string text)
        {
            if (_saveSelectionInfoField == null)
            {
                return;
            }

            _saveSelectionInfoField.text = text ?? string.Empty;
        }
    }
}
