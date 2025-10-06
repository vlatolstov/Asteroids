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
        private readonly Button _spawnBtn;
        private readonly Button _restartBtn;

        public event Action SpawnClicked;
        public event Action RestartClicked;

        public MainOverlayController(VisualElement root)
        {
            _root = root;
            _controls = root.Q<Label>("controls-label");
            _gameOver = root.Q<Label>("gameover-label");
            _finalScore = root.Q<Label>("final-score-label");
            _bestScore = root.Q<Label>("best-score-label");
            _spawnBtn = root.Q<Button>("spawn-player-btn");
            _restartBtn = root.Q<Button>("restart-game-btn");

            if (_spawnBtn != null)
            {
                _spawnBtn.clicked += () => SpawnClicked?.Invoke();
            }

            if (_restartBtn != null)
            {
                _restartBtn.clicked += () => RestartClicked?.Invoke();
            }
        }

        public void Preparing(string controlsText)
        {
            SetVisible(true);
            Set(_controls, true, controlsText);
            Set(_gameOver, false);
            Set(_finalScore, false);
            Set(_bestScore, false);
            Set(_spawnBtn, true, "Start");
            Set(_restartBtn, false);
        }

        public void Gameplay()
        {
            SetVisible(false);
        }

        public void GameOver(int finalScore, int bestScore)
        {
            SetVisible(true);
            Set(_controls, false);
            Set(_gameOver, true);
            Set(_finalScore, true, $"Final score: {finalScore}");
            Set(_bestScore, true, $"Best score: {bestScore}");
            Set(_spawnBtn, true, "BACK IN GAME");
            Set(_restartBtn, true);
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