using System;
using _Project.Runtime.Abstract.MVP;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class ShopView : BaseView
    {
        private UIDocument _doc;
        private VisualElement _root;
        private VisualElement _closingBackground;
        private VisualElement _shopWindow;

        public event Action BackgroundClicked;

        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
            _root = _doc.rootVisualElement;

            if (_root == null)
            {
                Debug.LogError("[ShopView] UIDocument rootVisualElement was null.");
                return;
            }

            _closingBackground = _root.Q<VisualElement>("shop-overlay-background");
            _shopWindow = _root.Q<VisualElement>("shop-window");

            if (_closingBackground == null)
            {
                Debug.LogError("[ShopView] Missing overlay background visual element.");
                return;
            }

            _closingBackground.RegisterCallback<ClickEvent>(OnBackgroundClicked);

            if (_shopWindow != null)
            {
                _shopWindow.RegisterCallback<ClickEvent>(OnShopWindowClicked);
            }

            Hide();
        }

        private void OnDestroy()
        {
            if (_closingBackground != null)
            {
                _closingBackground.UnregisterCallback<ClickEvent>(OnBackgroundClicked);
            }

            if (_shopWindow != null)
            {
                _shopWindow.UnregisterCallback<ClickEvent>(OnShopWindowClicked);
            }
        }

        public void Show()
        {
            if (_root == null)
            {
                return;
            }

            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            if (_root == null)
            {
                return;
            }

            _root.style.display = DisplayStyle.None;
        }

        private void OnBackgroundClicked(ClickEvent _)
        {
            BackgroundClicked?.Invoke();
        }

        private static void OnShopWindowClicked(ClickEvent evt)
        {
            evt.StopPropagation();
        }
    }
}
