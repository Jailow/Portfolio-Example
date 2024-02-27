using CaveMiner.Helpers;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UISellItemTab : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _priceText;

        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _infoButton;

        private ItemState _itemState;
        private UIManager _uiManager;
        private GameManager _gameManager;

        private const string CLOSE_KEY = "close";

        public void Init(GameManager gameManager, UIManager uiManager, Action<ItemState> onItemTake)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _infoButton.onClick.AddListener(ShowDescription);
            _sellButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                onItemTake?.Invoke(_itemState);
            });
        }

        private void ShowDescription()
        {
            if (_itemState == null)
                return;

            _uiManager.InfoPanel.Show($"Description/{_itemState.Id}", string.Empty, CLOSE_KEY, null, null, new string[0]);
        }

        public void SetItem(ItemState itemState)
        {
            _itemState = itemState;

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (itemData == null)
                return;

            var playerState = _gameManager.GameState.PlayerState;

            _itemIcon.sprite = itemData.Icon;
            _itemIcon.rectTransform.sizeDelta = CachedVector2.One * itemData.IconSize;

            var price = Mathf.Lerp(itemData.ShopPriceMin, itemData.ShopPriceMax, 0.5f);
            price *= _gameManager.GameData.SellItemPriceMultiplier;

            var sellPriceMultiplier = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "sell_price_multiplier");
            var sellPriceMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "sell_price_multiplier");

            price *= 1f + ((sellPriceMultiplier.Value * sellPriceMultiplierState.Level) * 0.01f);

            _priceText.text = NumberToString.Convert((int)price);
            _countText.text = itemState.Count.ToString();
            _nameText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{_itemState.Id}"), _itemState.CustomValue);

            _infoButton.gameObject.SetActive(itemData.HaveDescription);
        }

        private void OnEnable()
        {
            if (_itemState == null)
                return;

            _nameText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{_itemState.Id}"), _itemState.CustomValue);
        }
    }
}