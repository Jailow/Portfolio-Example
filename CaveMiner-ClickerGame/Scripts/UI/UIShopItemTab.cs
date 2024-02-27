using CaveMiner.Helpers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIShopItemTab : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _infoButton;

        private UIDisabledButtonAlpha _disabledButtonAlpha;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private ShopItemState _itemState;

        private const string CLOSE_KEY = "close";
        private int Price
        {
            get
            {
                var playerState = _gameManager.GameState.PlayerState;
                var gameShopPrices = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "game_shop_prices");
                var gameShopPricesState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "game_shop_prices");

                return (int)(_itemState.Price * (1f - (gameShopPrices.Value * gameShopPricesState.Level) / 100f));
            }
        }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _buyButton.onClick.AddListener(OnBuy);
            _infoButton.onClick.AddListener(ShowDescription);

            _disabledButtonAlpha = _buyButton.GetComponent<UIDisabledButtonAlpha>();
            _disabledButtonAlpha.Init();
        }

        private void ShowDescription()
        {
            if (_itemState == null)
                return;

            _uiManager.InfoPanel.Show($"Description/{_itemState.Id}", string.Empty, CLOSE_KEY, null, null, new string[0]);
        }

        private void OnBuy()
        {
            var playerState = _gameManager.GameState.PlayerState;

            if (_itemState.Count <= 0 || playerState.Money < Price)
                return;

            _uiManager.ButtonClickSound();

            _itemState.Count--;
            _gameManager.AddMoney(-Price);
            _gameManager.AddItem(_itemState.Id, 1, _itemState.CustomValue);
            playerState.Stats.BuyItemsInGameShop++;

            AchievementManager.Instance.CheckBuyItemsInGameShopCountCondition(playerState.Stats.BuyItemsInGameShop);

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("Item ID", _itemState.Id);
            AmplitudeManager.Instance.Event(AnalyticEventKey.BUY_GAME_SHOP_ITEM, properties);

            UpdateTab();
        }

        public void SetItem(ShopItemState itemState)
        {
            _itemState = itemState;

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (itemData == null)
                return;

            _itemIcon.sprite = itemData.Icon;
            _itemIcon.rectTransform.sizeDelta = CachedVector2.One * itemData.IconSize;

            _priceText.text = NumberToString.Convert(Price);
            _nameText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{_itemState.Id}"), _itemState.CustomValue);

            _infoButton.gameObject.SetActive(itemData.HaveDescription);
        }

        public void UpdateTab()
        {
            if (_itemState == null)
                return;

            var playerState = _gameManager.GameState.PlayerState;

            _countText.text = _itemState.Count.ToString();
            _disabledButtonAlpha.Interactable = playerState.Money >= Price && _itemState.Count > 0;
        }

        private void OnEnable()
        {
            _priceText.text = NumberToString.Convert(Price);
            _nameText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{_itemState.Id}"), _itemState.CustomValue);

            UpdateTab();
        }
    }
}