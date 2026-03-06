using System;
using System.Collections.Generic;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.InAppPurchase;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class ShopView : BaseView
    {
        private sealed class ShopCardRefs
        {
            public VisualElement Icon;
            public Label Title;
            public Label Description;
            public Label Price;
            public Button BuyButton;
            public ShopProductCardData BoundData;
        }

        private UIDocument _doc;
        private VisualElement _root;
        private VisualElement _closingBackground;
        private VisualElement _shopWindow;
        private ListView _shopItemsView;
        private Button _restorePurchasesButton;
        private Button _closeButton;

        private readonly List<ShopProductCardData> _products = new();

        public event Action BackgroundClicked;
        public event Action<string> PurchaseConfirmed;
        public event Action RestorePurchasesRequested;
        public event Action CloseRequested;

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
            _shopItemsView = _root.Q<ListView>("shop-items");
            _restorePurchasesButton = _root.Q<Button>("RestorePurchases");
            _closeButton = _root.Q<Button>("Close");

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

            if (_restorePurchasesButton != null)
            {
                _restorePurchasesButton.clicked += OnRestorePurchasesButtonClicked;
            }
            else
            {
                Debug.LogError("[ShopView] Missing RestorePurchases button.");
            }

            if (_closeButton != null)
            {
                _closeButton.clicked += OnCloseButtonClicked;
            }
            else
            {
                Debug.LogError("[ShopView] Missing Close button.");
            }

            if (_shopItemsView != null)
            {
                _shopItemsView.selectionType = SelectionType.None;
                _shopItemsView.fixedItemHeight = 108;
                _shopItemsView.itemsSource = _products;
                _shopItemsView.bindItem = BindItem;
                _shopItemsView.unbindItem = UnbindItem;
            }
            else
            {
                Debug.LogError("[ShopView] Missing shop items ListView.");
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

            if (_restorePurchasesButton != null)
            {
                _restorePurchasesButton.clicked -= OnRestorePurchasesButtonClicked;
            }

            if (_closeButton != null)
            {
                _closeButton.clicked -= OnCloseButtonClicked;
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

        public void SetProducts(IReadOnlyList<ShopProductCardData> products)
        {
            _products.Clear();

            if (products != null)
            {
                for (var i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    if (product == null)
                    {
                        continue;
                    }

                    _products.Add(product);
                }
            }

            _shopItemsView?.RefreshItems();
        }

        private void OnBackgroundClicked(ClickEvent _)
        {
            BackgroundClicked?.Invoke();
        }

        private static void OnShopWindowClicked(ClickEvent evt)
        {
            evt.StopPropagation();
        }

        private void OnRestorePurchasesButtonClicked()
        {
            RestorePurchasesRequested?.Invoke();
        }

        private void OnCloseButtonClicked()
        {
            CloseRequested?.Invoke();
        }

        private void BindItem(VisualElement element, int index)
        {
            if (index < 0 || index >= _products.Count)
            {
                return;
            }

            var refs = GetOrCreateRefs(element);
            if (refs == null)
            {
                return;
            }

            var data = _products[index];
            refs.BoundData = data;

            if (refs.Icon != null)
            {
                if (data.Icon != null)
                {
                    refs.Icon.style.backgroundImage = new StyleBackground(data.Icon);
                }
                else
                {
                    refs.Icon.style.backgroundImage = new StyleBackground();
                }
            }

            if (refs.Title != null)
            {
                refs.Title.text = data.Title;
            }

            if (refs.Description != null)
            {
                refs.Description.text = data.Description;
            }

            if (refs.Price != null)
            {
                refs.Price.text = data.Price;
            }

            if (refs.BuyButton != null)
            {
                refs.BuyButton.text = data.IsPurchased ? "Purchased" : "Buy";
                refs.BuyButton.SetEnabled(!data.IsPurchased);
            }
        }

        private void UnbindItem(VisualElement element, int _)
        {
            if (element.userData is ShopCardRefs refs)
            {
                refs.BoundData = null;
            }
        }

        private ShopCardRefs GetOrCreateRefs(VisualElement element)
        {
            if (element.userData is ShopCardRefs existingRefs)
            {
                return existingRefs;
            }

            var refs = new ShopCardRefs
            {
                Icon = element.Q<VisualElement>("shop-item-icon"),
                Title = element.Q<Label>("shop-item-title"),
                Description = element.Q<Label>("shop-item-description"),
                Price = element.Q<Label>("shop-item-price"),
                BuyButton = element.Q<Button>("shop-item-buy-btn")
            };

            if (refs.Icon == null ||
                refs.Title == null ||
                refs.Description == null ||
                refs.Price == null ||
                refs.BuyButton == null)
            {
                Debug.LogError("[ShopView] Item template is invalid. Missing required card elements.");
                return null;
            }

            refs.BuyButton.clicked += () => OnBuyButtonClicked(refs);

            element.userData = refs;
            return refs;
        }

        private void OnBuyButtonClicked(ShopCardRefs refs)
        {
            var data = refs?.BoundData;
            if (data == null)
            {
                return;
            }

            if (data.IsPurchased)
            {
                return;
            }

            PurchaseConfirmed?.Invoke(data.ProductId);
        }
    }
}
