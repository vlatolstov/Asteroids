using System;
using UnityEngine.UIElements;

namespace _Project.Runtime.UI
{
    public sealed class MainOverlayController
    {
        private readonly VisualElement _root;
        private readonly Label _controls;
        private readonly Label _gameOver;
        private readonly Label _finalScore;
        private readonly Label _bestScore;
        private readonly Button _respawnBtn;
        private readonly Button _backToMenuBtn;

        public event Action RespawnClicked;
        public event Action BackToMenuClicked;

        public MainOverlayController(VisualElement root)
        {
            _root = root;
            _controls = root.Q<Label>("controls-label");
            _gameOver = root.Q<Label>("gameover-label");
            _finalScore = root.Q<Label>("final-score-label");
            _bestScore = root.Q<Label>("best-score-label");
            _respawnBtn = root.Q<Button>("respawn-btn");
            _backToMenuBtn = root.Q<Button>("back-to-menu-btn");

            if (_respawnBtn != null)
            {
                _respawnBtn.clicked += () => RespawnClicked?.Invoke();
            }

            if (_backToMenuBtn != null)
            {
                _backToMenuBtn.clicked += () => BackToMenuClicked?.Invoke();
            }
        }

        public void Preparing(string controlsText)
        {
            SetVisible(true);
            Set(_controls, true, controlsText);
            Set(_gameOver, false);
            Set(_finalScore, false);
            Set(_bestScore, true);
            Set(_respawnBtn, false);
            Set(_backToMenuBtn, false);
        }

        public void Gameplay()
        {
            SetVisible(false);
            Set(_respawnBtn, false);
            Set(_backToMenuBtn, false);
        }

        public void GameOver(int finalScore, int bestScore, bool isNewRecord)
        {
            SetVisible(true);
            Set(_controls, false);
            Set(_gameOver, true);
            Set(_finalScore, true, $"Final score: {finalScore}");
            if (isNewRecord)
            {
                Set(_bestScore, true, $"New best score: {bestScore}!");
            }
            else
            {
                Set(_bestScore, false);
            }

            Set(_respawnBtn, true);
            Set(_backToMenuBtn, true);
        }

        public void SetBestScore(int bestScore)
        {
            if (_bestScore == null)
            {
                return;
            }

            _bestScore.text = $"Best score: {bestScore}";
        }

        private void SetVisible(bool v) => _root.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;

        private static void Set(Label lbl, bool visible, string text = null)
        {
            if (lbl == null)
            {
                return;
            }

            lbl.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (text != null)
            {
                lbl.text = text;
            }
        }

        private static void Set(Button btn, bool visible, string text = null)
        {
            if (btn == null)
            {
                return;
            }

            btn.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (text != null)
            {
                btn.text = text;
            }

            btn.AddToClassList("hud-button");
        }
    }
}
