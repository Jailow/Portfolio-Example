using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UISellItemsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _noItemsText;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private UISellItemTab _itemTabPrefab;

        private List<UIBackpackFilterButton> _filterButtons;
        private List<UISellItemTab> _itemTabs;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private ItemState _itemState;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _filterButtons = GetComponentsInChildren<UIBackpackFilterButton>(true).ToList();
            foreach (var btn in _filterButtons)
            {
                btn.Init(uiManager, UpdateItems);
            }

            _itemTabs = new List<UISellItemTab>();
        }

        private void OnTakeItem(ItemState itemState)
        {
            if (itemState.Count <= 0)
                return;

            _itemState = itemState;

            if(itemState.Count == 1)
            {
                SellItem(_itemState, 1);
            }
            else
            {
                _uiManager.ShowSelectCountPanel(1, _itemState.Count, OnCountSelected);
            }
        }

        private void OnCountSelected(int count)
        {
            SellItem(_itemState, count);
        }

        private void SellItem(ItemState itemState, int count)
        {
            var playerState = _gameManager.GameState.PlayerState;
            playerState.Stats.SellItemsCount += count;

            AchievementManager.Instance.CheckSellItemsCount(playerState.Stats.SellItemsCount);

            _gameManager.RemoveItemFromInventory(itemState.Id, count);

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);
            var price = Mathf.Lerp(itemData.ShopPriceMin, itemData.ShopPriceMax, 0.5f) * count;
            price *= _gameManager.GameData.SellItemPriceMultiplier;

            var sellPriceMultiplier = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "sell_price_multiplier");
            var sellPriceMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "sell_price_multiplier");

            price *= 1f + ((sellPriceMultiplier.Value * sellPriceMultiplierState.Level) * 0.01f);

            _gameManager.AddMoney((int)price);

            _itemState = null;

            UpdateVisual();
        }

        private void UpdateItems()
        {
            ClearItems();

            int currentIndex = 0;
            var playerState = _gameManager.GameState.PlayerState;

            playerState.Items.ForEach(e =>
            {
                var item = _gameManager.Items.FirstOrDefault(b => b.Id == e.Id);

                if (item != null && _filterButtons.Any(b => (b.FilterType == item.FilterType && b.IsSelected)))
                {
                    if (currentIndex > _itemTabs.Count - 1)
                    {
                        var itemTab = Instantiate(_itemTabPrefab, _scrollRect.content, false);
                        itemTab.Init(_gameManager, _uiManager, OnTakeItem);
                        _itemTabs.Add(itemTab);
                    }
                    else
                    {
                        _itemTabs[currentIndex].gameObject.SetActive(true);
                    }

                    _itemTabs[currentIndex].SetItem(e);

                    currentIndex++;
                }
            });

            _scrollRect.verticalNormalizedPosition = 1f;
        }

        private void ClearItems()
        {
            _itemTabs.ForEach(e => e.gameObject.SetActive(false));
        }

        private void UpdateVisual()
        {
            var playerState = _gameManager.GameState.PlayerState;

            _noItemsText.SetActive(playerState.Items.Count <= 0);

            UpdateItems();
        }

        private void OnEnable()
        {
            UpdateVisual();

            _itemState = null;
        }
    }
}