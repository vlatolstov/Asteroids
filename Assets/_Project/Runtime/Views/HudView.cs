using System;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class HudView : BaseView
    {
        private const string CONTROLS_TEXT = "W - forward, A - left, D - right.\n" +
                                             "Space - shoot gun. Ctrl - release laser.";

        private const string FINAL_SCORE_TEXT = "Final score: ";

        private UIDocument _doc;

        private VisualElement _shipDataContainer;
        private Label _posLabel;
        private Label _spdLabel;
        private Label _angLabel;
        private ProgressBar _rechargeProgressBar;
        private Label _laserChargesLabel;
        private Label _scoreLabel;

        private VisualElement _mainContainer;
        private Label _controlsLabel;
        private Label _gameOverLabel;
        private Label _finalScoreLabel;
        private Button _playerSpawnButton;
        private Button _restartGameButton;

        private GameState _gameState;

        public event Action PlayerSpawnButtonPressed;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();

            var root = _doc.rootVisualElement;

            _shipDataContainer = root.Q<VisualElement>("ShipDataContainer");
            _posLabel = root.Q<Label>("Pos");
            _spdLabel = root.Q<Label>("Speed");
            _angLabel = root.Q<Label>("Angle");
            _rechargeProgressBar = root.Q<ProgressBar>("Recharge");
            _laserChargesLabel = root.Q<Label>("LaserCharges");
            _scoreLabel = root.Q<Label>("Score");

            _mainContainer = root.Q<VisualElement>("MainContainer");
            _controlsLabel = root.Q<Label>("Controls");
            _controlsLabel.text = CONTROLS_TEXT;
            _gameOverLabel = root.Q<Label>("GameOverLabel");
            _finalScoreLabel = root.Q<Label>("FinalScore");
            _playerSpawnButton = root.Q<Button>("SpawnPlayer");
            _playerSpawnButton.clicked += OnSpawnPlayerButtonClicked;
            _restartGameButton = root.Q<Button>("RestartGame");
            _restartGameButton.clicked += OnRestartGameButtonClicked;
        }

        private void Preparing()
        {
            SetVisibility(_mainContainer, true);
            SetVisibility(_controlsLabel, true);
            _playerSpawnButton.text = "Start";

            SetVisibility(_shipDataContainer, false);
            SetVisibility(_gameOverLabel, false);
            SetVisibility(_finalScoreLabel, false);
            SetVisibility(_restartGameButton, false);
        }

        private void Gameplay()
        {
            SetVisibility(_shipDataContainer, true);

            SetVisibility(_mainContainer, false);
        }

        private void GameOver()
        {
            SetVisibility(_mainContainer, true);
            SetVisibility(_gameOverLabel, true);
            SetVisibility(_finalScoreLabel, true);
            _playerSpawnButton.text = "BACK IN GAME";
            SetVisibility(_playerSpawnButton, true);
            SetVisibility(_restartGameButton, true);

            SetVisibility(_shipDataContainer, false);
            SetVisibility(_controlsLabel, false);
        }

        public void UpdateGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Preparing:
                    Preparing();
                    break;
                case GameState.Gameplay:
                    Gameplay();
                    break;
                case GameState.GameOver:
                    GameOver();
                    break;
                case GameState.Pause:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdatePoseData(Vector2 pos, Vector2 vel, float angleRad)
        {
            if (_posLabel != null)
                _posLabel.text = $"POS X:{pos.x,7:0.00} Y:{pos.y,7:0.00}";

            if (_spdLabel != null)
            {
                float speed = vel.magnitude;
                _spdLabel.text = $"SPD {speed:0.00}";
            }

            if (_angLabel != null)
            {
                float angleDegrees = Mathf.Repeat(angleRad * Mathf.Rad2Deg, 360f);
                _angLabel.text = $"ANG {angleDegrees:0.0}°";
            }
        }

        public void UpdateProjectileWeaponData(float cooldown, float reloadRatio)
        { }

        public void UpdateAoeWeaponData(int total, int current, float rechargeRatio)
        {
            _rechargeProgressBar.visible = total != current;

            _rechargeProgressBar.value = rechargeRatio;

            _laserChargesLabel.text = $"Laser Charges: {current}";
        }

        public void UpdateScore(int score)
        {
            _scoreLabel.text = score.ToString();
            _finalScoreLabel.text = FINAL_SCORE_TEXT + score;
        }

        private void OnSpawnPlayerButtonClicked()
        {
            PlayerSpawnButtonPressed?.Invoke();
        }

        private void OnRestartGameButtonClicked()
        {
            //TODO replace 
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }

        private void SetVisibility(VisualElement elem, bool visible)
        {
            if (elem == null)
            {
                return;
            }

            elem.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}