using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace CaveMiner
{
    public class IAPController : Singleton<IAPController>, IDetailedStoreListener
    {
        private IStoreController _controller;
        private IExtensionProvider _extensions;

        public Action<Product> onPurchaseCompleted;
        public Action onPurchaseRestored;

        public IStoreController Controller => _controller;
        public bool IsInitialized { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            IsInitialized = false;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct("5000_minerals", ProductType.Consumable);
            builder.AddProduct("10000_minerals", ProductType.Consumable);
            builder.AddProduct("50000_minerals", ProductType.Consumable);
            builder.AddProduct("2_epic_keys", ProductType.Consumable);
            builder.AddProduct("2_mythical_keys", ProductType.Consumable);
            builder.AddProduct("2_legendary_keys", ProductType.Consumable);
            builder.AddProduct("no_ads", ProductType.NonConsumable);
            builder.AddProduct("recycle_x5", ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void RestorePurchase()
        {
            _extensions.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions((b, arg) =>
            {
                if (b)
                {
                    Debug.Log("Purchase Restored");
                    onPurchaseRestored?.Invoke();
                }
                else
                {
                    // Restoration failed.
                }
            });
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            IsInitialized = true;

            _controller = controller;
            _extensions = extensions;

            RestorePurchase();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"IAPController InitializeFailed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log($"IAPController InitializeFailed: {error} - {message}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"IAPController PurchaseFailed: {product.definition} - {failureReason}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;

            Debug.Log("Purchase Completed: " + product.definition.id);

            onPurchaseCompleted?.Invoke(product);

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"IAPController PurchaseFailed: {product.definition} - {failureDescription.productId} : {failureDescription.message} : {failureDescription.reason}");
        }

        public void Purchase(string id)
        {
            if (!_controller.products.WithID(id).availableToPurchase)
                return;

            _controller.InitiatePurchase(id);
        }

        public ProductMetadata GetPurchaseData(string id)
        {
            return _controller.products.WithID(id).metadata;
        }
    }
}