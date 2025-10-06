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

        private const float LargeWidthThreshold = 1280f;
        private const float ExtraLargeWidthThreshold = 1600f;
        private const float CompactWidthThreshold = 960f;
        private const float ExtraCompactWidthThreshold = 720f;
        private const float ShortHeightThreshold = 700f;
        private const float UltraShortHeightThreshold = 560f;

        private UIDocument _doc;
        private VisualElement _root;

        private MainOverlayController _overlay;
        private ShipDataController _shipData;
        private WeaponIconController _projectileWeapon;
        private WeaponIconController _aoeWeapon;
        private VisualElement _weaponsPanel;
        private VisualElement _statsPanel;
        private Label _statsContent;

        private string _statisticsSummary = string.Empty;
        private bool _statsPanelShouldBeVisible;
        private int _score;
        private int _bestScore;

        public event Action PlayerSpawnButtonPressed;
        public event Action RestartButtonPressed;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
            _root = _doc.rootVisualElement;

            if (_root == null)
            {
                Debug.LogError("[HudView] UIDocument rootVisualElement was null.");
                return;
            }

            _root.RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
            _root.schedule.Execute(() => ApplyResponsiveClasses(_root.contentRect));

            _overlay = new MainOverlayController(_root.Q<VisualElement>("main-container"));
            _shipData = new ShipDataController(_root.Q<VisualElement>("ship-data"));
            _weaponsPanel = _root.Q<VisualElement>("weapons-panel");
            _statsPanel = _root.Q<VisualElement>("stats-panel");
            _statsContent = _statsPanel?.Q<Label>("stats-content");

            _projectileWeapon = new WeaponIconController(_weaponsPanel.Q<VisualElement>("projectile-weapon-icon"));
            _projectileWeapon.SetCountVisible(false);
            _projectileWeapon.SetReloadVisible(false);

            _aoeWeapon = new WeaponIconController(_weaponsPanel.Q<VisualElement>("aoe-weapon-icon"));

            _overlay.SpawnClicked += () => PlayerSpawnButtonPressed?.Invoke();
            _overlay.RestartClicked += RestartSceneNow;

            UpdateStatsPanelVisibility(false);
        }

        private void OnDestroy()
        {
            if (_root != null)
            {
                _root.UnregisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
            }
        }

        private void HandleGeometryChanged(GeometryChangedEvent evt)
        {
            ApplyResponsiveClasses(evt.newRect);
        }

        private void ApplyResponsiveClasses(Rect rect)
        {
            if (_root == null)
            {
                return;
            }

            float width = rect.width;
            float height = rect.height;

            if (width <= 0f || height <= 0f)
            {
                return;
            }

            bool isPortrait = height > width;
            ToggleRootClass("hud--portrait", isPortrait);
            ToggleRootClass("hud--landscape", !isPortrait);

            bool isExtraLarge = width >= ExtraLargeWidthThreshold;
            bool isLarge = width >= LargeWidthThreshold && width < ExtraLargeWidthThreshold;
            bool isCompact = width < CompactWidthThreshold && width >= ExtraCompactWidthThreshold;
            bool isExtraCompact = width < ExtraCompactWidthThreshold;

            ToggleRootClass("hud--extra-large", isExtraLarge);
            ToggleRootClass("hud--large", isLarge);
            ToggleRootClass("hud--compact", isCompact);
            ToggleRootClass("hud--extra-compact", isExtraCompact);

            bool isUltraShort = height < UltraShortHeightThreshold;
            bool isShort = height < ShortHeightThreshold && height >= UltraShortHeightThreshold;

            ToggleRootClass("hud--ultra-short", isUltraShort);
            ToggleRootClass("hud--short", isShort);
        }

        private void ToggleRootClass(string className, bool enable)
        {
            if (_root == null)
            {
                return;
            }

            _root.EnableInClassList(className, enable);
        }

        public void UpdateGameState(GameState state)
        {
            switch (state)
            {
                case GameState.Preparing:
                    _statisticsSummary = string.Empty;
                    UpdateStatsPanelVisibility(false);
                    _overlay.Preparing(CONTROLS_TEXT);
                    _shipData.SetVisible(false);
                    _weaponsPanel.style.display = DisplayStyle.None;
                    break;
                case GameState.Gameplay:
                    UpdateStatsPanelVisibility(false);
                    _overlay.Gameplay();
                    _shipData.SetVisible(true);
                    _weaponsPanel.style.display = DisplayStyle.Flex;
                    break;
                case GameState.GameOver:
                    UpdateStatsPanelVisibility(true);
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

        public void SetStatisticsSummary(string summary)
        {
            _statisticsSummary = string.IsNullOrWhiteSpace(summary) ? string.Empty : summary;

            if (_statsContent != null)
            {
                _statsContent.text = _statisticsSummary;
            }

            UpdateStatsPanelVisibility(_statsPanelShouldBeVisible);
        }

        private void UpdateStatsPanelVisibility(bool visible)
        {
            _statsPanelShouldBeVisible = visible;

            if (_statsPanel == null)
            {
                return;
            }

            bool hasContent = !string.IsNullOrWhiteSpace(_statisticsSummary);
            if (!visible || !hasContent)
            {
                _statsPanel.style.display = DisplayStyle.None;
                return;
            }

            if (_statsContent != null)
            {
                _statsContent.text = _statisticsSummary;
            }

            _statsPanel.style.display = DisplayStyle.Flex;
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