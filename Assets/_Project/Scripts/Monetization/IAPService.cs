using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace Asteroids
{
    public class IAPService : IInitializable, IStoreService, IDisposable
    {
        private StoreController _storeController;
        private Dictionary<string, ProductType> _products;
        private IAPProductList _productList;

        public Dictionary<string, bool> Entitlements { get; private set; }

        [Inject]
        public void Construct(IAPProductList productList)
        {
            _productList = productList;
        }

        public void Initialize()
        {
            _storeController = UnityIAPServices.StoreController();

            _storeController.OnPurchasePending += OnPurchasePending;
            _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            _storeController.OnCheckEntitlement += OnCheckEntitlement;
            _storeController.OnStoreDisconnected += OnStoreDisconnected;

            StartIAPProcess().Forget();
        }

        private async UniTaskVoid StartIAPProcess()
        {
            await _storeController.Connect().AsUniTask();

            _products = _productList.Products;
            Entitlements = new Dictionary<string, bool>();

            foreach (var product in _products)
            {
                Entitlements[product.Key] = false;
            }

            FetchProducts();
        }

        public void Dispose()
        {
            _storeController.OnPurchasePending -= OnPurchasePending;
            _storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
            _storeController.OnCheckEntitlement -= OnCheckEntitlement;
            _storeController.OnStoreDisconnected -= OnStoreDisconnected;
            _storeController.OnProductsFetched -= OnProductsFetched;
            _storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
        }

        public void BuyProduct(string id)
        {
            _storeController.PurchaseProduct(id);
        }

        public bool CheckProductStatus(string id)
        {
            if (Entitlements[id] == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FetchProducts()
        {
            var products = new List<ProductDefinition>();

            foreach (var id in _products)
            {
                products.Add(new ProductDefinition(id.Key, id.Value));
            }

            _storeController.OnProductsFetched += OnProductsFetched;
            _storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            _storeController.FetchProducts(products);
        }

        private void OnProductsFetched(List<UnityEngine.Purchasing.Product> list)
        {
            Debug.Log("Products successfully fetched from the store.");
            CheckEntitlements();
        }

        private void OnProductsFetchFailed(ProductFetchFailed failed)
        {
            Debug.Log($"Fetch failed. Error: {failed.FailureReason}");
        }

        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            Debug.Log($"Store disconnected. Error: {description}");
        }

        private void OnCheckEntitlement(Entitlement entitlement)
        {
            var id = entitlement.Product.definition.id;

            if (Entitlements.ContainsKey(id))
            {
                switch (entitlement.Status)
                {
                    case EntitlementStatus.FullyEntitled:
                        Entitlements[id] = true;                        
                        break;
                    default:
                        Entitlements[id] = false;
                        break;
                }
            }
        }

        private void OnPurchaseConfirmed(Order order)
        {
            CheckEntitlements();
        }

        private void OnPurchasePending(PendingOrder order)
        {
            _storeController.ConfirmPurchase(order);
        }

        private void CheckEntitlements()
        {
            foreach (var item in _products)
            {
                var product = _storeController.GetProducts().FirstOrDefault(p => p.definition.id == item.Key);
                _storeController.CheckEntitlement(product);
            }
        }
    }
}