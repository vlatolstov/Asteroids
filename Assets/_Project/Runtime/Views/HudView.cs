using System;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;
using _Project.Runtime.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class HudView : BaseView
    {
        private const string CONTROLS_TEXT =
            "W - forward, A - left, D - right.\nSpace - shoot gun. Ctrl - release laser.";

        private UIDocument _doc;

        private MainOverlayController _overlay;
        private ShipDataController _shipData;
        private WeaponIconController _projectileWeapon;
        private WeaponIconController _aoeWeapon;
        private VisualElement _weaponsPanel;

        private int _score;
        private int _bestScore;

        public event Action PlayerSpawnButtonPressed;
        public event Action RestartButtonPressed;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
            var root = _doc.rootVisualElement;

            _overlay = new MainOverlayController(root.Q<VisualElement>("main-container"));

            _shipData = new ShipDataController(root.Q<VisualElement>("ship-data"));

            _weaponsPanel = root
                .Q<VisualElement>("weapons-panel");
            
            _projectileWeapon = new WeaponIconController(_weaponsPanel
                .Q<VisualElement>("projectile-weapon-icon"));
            _projectileWeapon.SetCountVisible(false);
            _projectileWeapon.SetReloadVisible(false);

            _aoeWeapon = new WeaponIconController(_weaponsPanel
                .Q<VisualElement>("aoe-weapon-icon"));

            _overlay.SpawnClicked += () => PlayerSpawnButtonPressed?.Invoke();
            _overlay.RestartClicked += RestartSceneNow;
        }

        public void UpdateGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Preparing:
                    _overlay.Preparing(CONTROLS_TEXT);
                    _shipData.SetVisible(false);
                    _weaponsPanel.style.display = DisplayStyle.None;
                    break;
                case GameState.Gameplay:
                    _overlay.Gameplay();
                    _shipData.SetVisible(true);
                    _weaponsPanel.style.display = DisplayStyle.Flex;
                    break;
                case GameState.GameOver:
                    _overlay.GameOver(_score, _bestScore);
                    _shipData.SetVisible(false);
                    _weaponsPanel.style.display = DisplayStyle.None;
                    break;
                case GameState.Pause:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void UpdatePoseData(Vector2 pos, Vector2 vel, float angleRad) =>
            _shipData.UpdatePose(pos, vel, angleRad);


        public void UpdateScore(int score)
        {
            _shipData.UpdateScore(score);
            _score = score;
        }

        public void UpdateBestScore(int bestScore)
        {
            _bestScore = bestScore;
            _overlay.SetBestScore(bestScore);
        }

        private void RestartSceneNow()
        {
            //TODO remove
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
            //
            RestartButtonPressed?.Invoke();
        }

        public void UpdateProjectileWeaponData(float cooldown, float reloadRatio)
        {
            _projectileWeapon.UpdateCooldownState(reloadRatio);
        }
        
        public void UpdateAoeWeaponData(int total, int current, float rechargeRatio, float cooldown, float reloadRatio)
        {
            _aoeWeapon.SetReloadVisible(total != current);
            _aoeWeapon.SetReloadProgress01(Mathf.Clamp01(rechargeRatio));
            _aoeWeapon.SetCount(current);
            
            _aoeWeapon.UpdateCooldownState(reloadRatio);
        }
        
        public void SetAoeWeaponIcon(Sprite sprite) => _aoeWeapon.SetIcon(sprite);
        public void SetProjectileWeaponIcon(Sprite sprite) => _projectileWeapon.SetIcon(sprite);
    }
}
