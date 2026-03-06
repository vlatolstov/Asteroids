using System;
using _Project.Runtime.Abstract.MVP;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class MenuView : BaseView
    {
        private UIDocument _doc;
        private VisualElement _root;
        private Button _startButton;
        private Button _shopButton;
        private Button _exitButton;
        private Label _bestScoreLabel;

        public event Action StartButtonClicked;
        public event Action ShopButtonClicked;
        public event Action ExitButtonClicked;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
            _root = _doc.rootVisualElement;

            if (_root == null)
            {
                Debug.LogError("[MenuView] UIDocument rootVisualElement was null.");
                return;
            }

            _startButton = _root.Q<Button>("start-game-btn");
            _shopButton = _root.Q<Button>("shop-btn");
            _exitButton = _root.Q<Button>("exit-game-btn");
            _bestScoreLabel = _root.Q<Label>("best-score-label");

            if (_startButton != null)
            {
                _startButton.clicked += OnStartButtonClicked;
            }

            if (_shopButton != null)
            {
                _shopButton.clicked += OnShopButtonClicked;
            }

            if (_exitButton != null)
            {
                _exitButton.clicked += OnExitButtonClicked;
            }
        }

        private void OnDestroy()
        {
            if (_startButton != null)
            {
                _startButton.clicked -= OnStartButtonClicked;
            }

            if (_shopButton != null)
            {
                _shopButton.clicked -= OnShopButtonClicked;
            }

            if (_exitButton != null)
            {
                _exitButton.clicked -= OnExitButtonClicked;
            }
        }

        public void SetBestScore(int score)
        {
            if (_bestScoreLabel == null)
            {
                return;
            }

            _bestScoreLabel.text = $"Best score: {score}";
        }

        private void OnStartButtonClicked()
        {
            StartButtonClicked?.Invoke();
        }

        private void OnShopButtonClicked()
        {
            ShopButtonClicked?.Invoke();
        }

        private void OnExitButtonClicked()
        {
            ExitButtonClicked?.Invoke();
        }
    }
}
