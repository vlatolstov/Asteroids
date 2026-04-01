using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Runtime.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Project.Runtime.InAppPurchase
{
    public class UnityIapService : IIapService
    {
        private readonly StoreController _storeController;
        private readonly PlayerModel _playerModel;
        private readonly Dictionary<string, ProductType> _catalogProductTypesById = new(StringComparer.Ordinal);

        public IReadOnlyList<Product> Products { get; private set; } = new List<Product>();
        public event Action PurchasesChanged;

        public UnityIapService(PlayerModel playerModel)
        {
            _playerModel = playerModel;
            LoadCatalogProductTypes();

            _storeController = UnityIAPServices.StoreController();

            _storeController.OnStoreDisconnected += OnStoreDisconnected;

            _storeController.OnProductsFetched += OnProductsFetched;
            _storeController.OnProductsFetchFailed += OnProductsFetchFailed;

            _storeController.OnPurchasesFetched += OnPurchasesFetched;
            _storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            _storeController.OnPurchasePending += OnPurchasePending;
            _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            _storeController.OnPurchaseFailed += OnPurchaseFailed;
            _storeController.OnPurchaseDeferred += OnPurchaseDeferred;
        }

        private void OnStoreDisconnected(StoreConnectionFailureDescription failure)
        {
            Debug.LogWarning(failure.Message);
        }

        private void OnProductsFetched(List<Product> products)
        {
            Products = products ?? new List<Product>();

            Debug.Log($"Fetched {Products.Count} products: {string.Join(',', Products)}");
            PurchasesChanged?.Invoke();
            _storeController.FetchPurchases();
        }

        private void OnProductsFetchFailed(ProductFetchFailed failure)
        {
            Debug.LogWarning(failure.FailureReason);
        }

        private void OnPurchasesFetched(Orders orders)
        {
            var changed = RefreshEntitlementsFromOrders(orders);
            if (changed)
            {
                PurchasesChanged?.Invoke();
            }

            var confirmed = orders?.ConfirmedOrders?.Count ?? 0;
            var deferred = orders?.DeferredOrders?.Count ?? 0;
            var pending = orders?.PendingOrders?.Count ?? 0;
            Debug.Log($"Total {confirmed + deferred + pending} orders");
        }

        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
        {
            Debug.LogWarning(failure.Message);
        }

        private void OnPurchasePending(PendingOrder order)
        {
            var changed = CachePurchasedProductsFromOrder(order, includeConsumables: false);
            if (changed)
            {
                PurchasesChanged?.Invoke();
            }

            _storeController.ConfirmPurchase(order);
        }

        private void OnPurchaseConfirmed(Order order)
        {
            var changed = CachePurchasedProductsFromOrder(order, includeConsumables: true);
            if (changed)
            {
                PurchasesChanged?.Invoke();
            }
        }

        private static void OnPurchaseFailed(FailedOrder order)
        {
            var productId = order?.CartOrdered?.Items()?.FirstOrDefault()?.Product?.definition?.id;
            Debug.LogWarning($"Purchase failed for product '{productId}'.");
        }

        private static void OnPurchaseDeferred(DeferredOrder order)
        {
            var productId = order?.CartOrdered?.Items()?.FirstOrDefault()?.Product?.definition?.id;
            Debug.Log($"Purchase deferred for product '{productId}'.");
        }

        public async UniTask Connect()
        {
            await _storeController.Connect();
        }

        public void FetchProducts()
        {
            var catalog = ProductCatalog.LoadDefaultCatalog();
            var provider = CodelessCatalogProvider.PopulateCatalogProvider(catalog);

            provider.FetchProducts(
                _storeController.FetchProductsWithNoRetries,
                DefaultStoreHelper.GetDefaultStoreName()
            );
        }

        public void Purchase(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                Debug.LogWarning("[IAP] Purchase was called with empty product id.");
            }

            var product = Products.FirstOrDefault(item => item.definition?.id == productId);
            if (product == null)
            {
                Debug.LogWarning($"[IAP] Product '{productId}' not found in fetched products.");

                var productType = ResolveProductType(productId, product);
                if (productType != ProductType.Consumable &&
                    _playerModel.CheckEntitlement(productId, productType))
                {
                    Debug.Log($"[IAP] Product '{productId}' is already owned.");
                }

                if (!product.availableToPurchase)
                {
                    Debug.LogWarning($"[IAP] Product '{productId}' is not available to purchase.");
                }

                try
                {
                    _storeController.PurchaseProduct(product);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"[IAP] Purchase failed to start for '{productId}'. {exception.Message}");
                }
            }
        }

        public bool RestoreTransactions()
        {
            try
            {
                _storeController.RestoreTransactions((result, message) =>
                {
                    if (result)
                    {
                        _storeController.FetchPurchases();
                        return;
                    }

                    Debug.LogWarning($"[IAP] Restore transactions failed. {message}");
                });

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[IAP] Restore transactions request failed. {exception.Message}");
                return false;
            }
        }

        private bool RefreshEntitlementsFromOrders(Orders orders)
        {
            var nonConsumableProductIds = new HashSet<string>(StringComparer.Ordinal);
            var activeSubscriptionProductIds = new HashSet<string>(StringComparer.Ordinal);

            var confirmedOrders = orders?.ConfirmedOrders;
            if (confirmedOrders != null)
            {
                for (var i = 0; i < confirmedOrders.Count; i++)
                {
                    CollectEntitlementsFromOrder(confirmedOrders[i], nonConsumableProductIds, activeSubscriptionProductIds);
                }
            }

            var pendingOrders = orders?.PendingOrders;
            if (pendingOrders != null)
            {
                for (var i = 0; i < pendingOrders.Count; i++)
                {
                    CollectEntitlementsFromOrder(pendingOrders[i], nonConsumableProductIds, activeSubscriptionProductIds);
                }
            }

            
            if (nonConsumableProductIds.Count == 0 && activeSubscriptionProductIds.Count == 0)
            {
                Debug.Log("[IAP] Store returned no entitlement orders. Keeping local entitlements.");
                return false;
            }

            return _playerModel.ReplaceEntitlements(nonConsumableProductIds, activeSubscriptionProductIds);
        }

        private bool CachePurchasedProductsFromOrder(Order order, bool includeConsumables)
        {
            var changed = false;
            var cartItems = order?.CartOrdered?.Items();
            if (cartItems == null)
            {
                return false;
            }

            for (var i = 0; i < cartItems.Count; i++)
            {
                var product = cartItems[i]?.Product;
                var productId = product?.definition?.id;
                if (string.IsNullOrWhiteSpace(productId))
                {
                    continue;
                }

                var productType = ResolveProductType(productId, product);
                if (productType == ProductType.Consumable && !includeConsumables)
                {
                    continue;
                }

                changed |= _playerModel.TryRegisterPurchase(productId, productType, includeConsumables);
            }

            return changed;
        }

        private void CollectEntitlementsFromOrder(Order order,
            ISet<string> nonConsumableProductIds,
            ISet<string> activeSubscriptionProductIds)
        {
            var cartItems = order?.CartOrdered?.Items();
            if (cartItems == null)
            {
                return;
            }

            for (var i = 0; i < cartItems.Count; i++)
            {
                var product = cartItems[i]?.Product;
                var productId = product?.definition?.id;
                if (string.IsNullOrWhiteSpace(productId))
                {
                    continue;
                }

                var productType = ResolveProductType(productId, product);
                switch (productType)
                {
                    case ProductType.Subscription:
                        activeSubscriptionProductIds.Add(productId);
                        break;
                    case ProductType.Consumable:
                        break;
                    case ProductType.NonConsumable:
                    case ProductType.Unknown:
                    default:
                        nonConsumableProductIds.Add(productId);
                        break;
                }
            }
        }

        private ProductType ResolveProductType(string productId, Product product = null)
        {
            if (product?.definition != null && product.definition.type != ProductType.Unknown)
            {
                return product.definition.type;
            }

            for (var i = 0; i < Products.Count; i++)
            {
                var current = Products[i];
                if (current?.definition == null)
                {
                    continue;
                }

                if (current.definition.id == productId)
                {
                    return current.definition.type;
                }
            }

            if (_catalogProductTypesById.TryGetValue(productId, out var catalogProductType))
            {
                return catalogProductType;
            }

            return ProductType.Unknown;
        }

        private void LoadCatalogProductTypes()
        {
            try
            {
                var catalog = ProductCatalog.LoadDefaultCatalog();
                var catalogItems = catalog?.allProducts;
                if (catalogItems == null)
                {
                    return;
                }

                foreach (var product in catalogItems)
                {
                    if (product == null || string.IsNullOrWhiteSpace(product.id))
                    {
                        continue;
                    }

                    _catalogProductTypesById[product.id] = product.type;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[IAP] Failed to load product catalog for product type mapping. {exception.Message}");
            }
        }
    }
}
