using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIBackpackPanel : MonoBehaviour
    {
        [SerializeField] private UIInventoryItemTab _inventoryItemTabPrefab;
        [SerializeField] private ScrollRect _scrollRect;

        private List<UIBackpackFilterButton> _filterButtons;
        private List<UIInventoryItemTab> _itemTabs;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _filterButtons = GetComponentsInChildren<UIBackpackFilterButton>(true).ToList();
            foreach(var btn in _filterButtons)
            {
                btn.Init(uiManager, UpdateItems);
            }

            _itemTabs = new List<UIInventoryItemTab>();
        }

        private void ClearItems()
        {
            _itemTabs.ForEach(e => e.gameObject.SetActive(false));
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
                        var itemTab = Instantiate(_inventoryItemTabPrefab, _scrollRect.content, false);
                        itemTab.Init(_gameManager, _uiManager, null);
                        itemTab.FadeActive = false;
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

        private void OnEnable()
        {
            UpdateItems();
        }
    }
}