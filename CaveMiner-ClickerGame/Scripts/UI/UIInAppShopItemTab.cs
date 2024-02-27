using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

namespace CaveMiner.UI
{
    public class UIInAppShopItemTab : MonoBehaviour
    {
        [SerializeField] private string _productId;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private Button _buyButton;

        private void Awake()
        {
            var productData = IAPController.Instance.Controller.products.WithID(_productId);
            if (productData != null && productData.hasReceipt && productData.definition.type == ProductType.NonConsumable)
            {
                gameObject.SetActive(false);
                return;
            }

            _buyButton.onClick.AddListener(() =>
            {
                IAPController.Instance.Purchase(_productId);
            });
        }

        private void OnPurchaseCompleted(Product product)
        {
            if (product.definition.id == _productId && product.definition.type == ProductType.NonConsumable)
                gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IAPController.Instance.onPurchaseCompleted += OnPurchaseCompleted;

            var product = IAPController.Instance.Controller.products.WithID(_productId);

            if(product.definition.type == ProductType.NonConsumable && product.hasReceipt)
            {
                gameObject.SetActive(false);
            }

            _name.text = product.metadata.localizedTitle;
            _price.text = product.metadata.localizedPriceString;
        }

        private void OnDisable()
        {
            IAPController.Instance.onPurchaseCompleted -= OnPurchaseCompleted;
        }
    }
}