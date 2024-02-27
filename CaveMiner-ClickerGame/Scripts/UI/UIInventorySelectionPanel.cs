using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIInventorySelectionPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private UIInventoryItemTab _inventoryItemTabPrefab;
        [SerializeField] private ScrollRect _scrollRect;

        private List<UIBackpackFilterButton> _filterButtons;
        private UICategoryButton _currentCategoryButton;
        private List<UIInventoryItemTab> _itemTabs;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private Action<ItemState> _onItemTake;
        private ItemType[] _itemTypes;
        private string[] _itemIds;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _itemTabs = new List<UIInventoryItemTab>();

            _filterButtons = GetComponentsInChildren<UIBackpackFilterButton>(true).ToList();
            foreach (var btn in _filterButtons)
            {
                btn.Init(uiManager, UpdateItems);
            }

            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        public void SelectCategory(UICategoryButton button)
        {
            if (_currentCategoryButton == button)
                return;

            _currentCategoryButton?.Deselect();
            _currentCategoryButton = button;
            button.Select();

            UpdateItems();
        }

        private void ClearItems()
        {
            _itemTabs.ForEach(e => e.gameObject.SetActive(false));
        }

        private void OnItemTake(ItemState itemState)
        {
            gameObject.SetActive(false);
            _onItemTake?.Invoke(itemState);
        }

        public void Show(Action<ItemState> onItemTake, ItemType[] itemTypes, string[] itemIds)
        {
            _itemIds = itemIds;
            _itemTypes = itemTypes;
            _onItemTake = onItemTake;
            gameObject.SetActive(true);

            UpdateItems();
        }

        private void UpdateItems()
        {
            ClearItems();

            int currentIndex = 0;
            var playerState = _gameManager.GameState.PlayerState;

            var category = _currentCategoryButton as UIInventoryCategoryButton;

            playerState.Items.ForEach(e =>
            {
                var item = _gameManager.Items.FirstOrDefault(b => b.Id == e.Id);

                if (item != null && _filterButtons.Any(b => (b.FilterType == item.FilterType && b.IsSelected)))
                {
                    if (currentIndex > _itemTabs.Count - 1)
                    {
                        var itemTab = Instantiate(_inventoryItemTabPrefab, _scrollRect.content, false);
                        itemTab.Init(_gameManager, _uiManager, OnItemTake);
                        _itemTabs.Add(itemTab);
                    }
                    else
                    {
                        _itemTabs[currentIndex].gameObject.SetActive(true);
                    }

                    _itemTabs[currentIndex].SetSelectionItem(e);

                    bool fadeActive = true;
                    if (_itemTypes.Any(b => b == item.ItemType))
                    {
                        fadeActive = false;
                    }
                    else
                    {
                        if (_itemIds != null && _itemIds.Length > 0)
                        {
                            fadeActive = !_itemIds.Any(b => b == item.Id);
                        }
                    }

                    _itemTabs[currentIndex].FadeActive = fadeActive;

                    currentIndex++;
                }
            });

            _scrollRect.verticalNormalizedPosition = 1f;
        }

        private void OnEnable()
        {
            IsOpened = true;
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}