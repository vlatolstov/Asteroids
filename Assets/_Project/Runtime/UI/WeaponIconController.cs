using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Runtime.UI
{
    public class WeaponIconController
    {
        private readonly VisualElement _root;
        private readonly VisualElement _icon;
        private readonly VisualElement _reloadMask;
        private readonly VisualElement _reloadFill;
        private readonly Label _count;

        public WeaponIconController(VisualElement root)
        {
            _root = root;
            _icon = _root.Q<VisualElement>("weapon-icon");
            _reloadMask = root.Q<VisualElement>("reload-mask");
            _reloadFill = root.Q<VisualElement>("reload-fill");
            _count = root.Q<Label>("count");
        }

        public void SetIcon(Texture2D tex) => _icon.style.backgroundImage = new StyleBackground(tex);
        public void SetIcon(Sprite sprite) => _icon.style.backgroundImage = new StyleBackground(sprite);

        public void SetReloadVisible(bool visible) =>
            _reloadMask.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        public void SetReloadProgress01(float t)
        {
            t = Mathf.Clamp01(t);
            _reloadFill.style.height = Length.Percent(t * 100f);
        }

        public void SetCountVisible(bool visible) =>
            _count.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        public void SetCount(int value) => _count.text = value.ToString();

        public void UpdateCooldownState(float state,
            float minOpacity = 0.1f, float maxOpacity = 0.6f)
        {
            const float epsilon = 0.001f;
            if (state < epsilon)
            {
                _root.style.opacity = 1f;
                return;
            }
            
            _root.style.opacity = Mathf.Lerp(minOpacity, maxOpacity, state);
        }
    }
}