using UnityEngine;
using TMPro;
using CaveMiner.Helpers;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CaveMiner.UI
{
    public class UICraftPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentExperiencePerSecond;
        [SerializeField] private UICraftItemTab _craftItemTabPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private int _maxVisibleTabCount;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private List<UICraftItemTab> _itemTabs;
        private int _index;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _itemTabs = new List<UICraftItemTab>();

            UpdateItems();
        }

        private void UpdateItems()
        {
            _index = 0;
            var playerState = _gameManager.GameState.PlayerState;

            foreach(var tab in _itemTabs)
            {
                tab.gameObject.SetActive(false);
            }

            //for (int i = 0; i < _gameManager.CraftItems.Length; i++)
            //{
            //    if (i >= _gameManager.CraftItems.Length)
            //        break;

            //    if (playerState.CraftedItemsIds.Contains(_gameManager.CraftItems[i].Id))
            //        continue;

            //    if (_index >=_itemTabs.Count)
            //    {
            //        var newTab = Instantiate(_craftItemTabPrefab, _scrollRect.content, false);
            //        newTab.Init(_gameManager, _uiManager, OnItemCrafted);
            //        _itemTabs.Add(newTab);
            //    }

            //    var tab = _itemTabs[_index];
            //    tab.gameObject.SetActive(true);
            //    tab.transform.SetSiblingIndex(i);
            //    tab.Set(_gameManager.CraftItems[i]);
            //    _index++;

            //    if (_index >= _maxVisibleTabCount)
            //        break;
            //}
        }

        public void OnItemCrafted(UICraftItemTab tab, CraftItemData craftItemData)
        {
            var playerState = _gameManager.GameState.PlayerState;

            playerState.CraftedItemsIds.Add(craftItemData.Id);
            //playerState.Stats.ItemsCrafted++;

            //AchievementManager.Instance.CheckCraftCountCondition(playerState.Stats.ItemsCrafted);

            //_gameManager.AddExperiencePerSecond(craftItemData.ExperiencePerSecond);
            _gameManager.AddResourceItem(craftItemData.ResourceToCraft.Id, -craftItemData.CountToCraft);

            //if (_index < _gameManager.CraftItems.Length)
            //{
            //    tab.Set(_gameManager.CraftItems[_index]);
            //    tab.transform.SetSiblingIndex(int.MaxValue);
            //    _index++;
            //}
            //else
            //{
            //    tab.gameObject.SetActive(false);
            //}
        }

        private void UpdateExperiencePerSecond(int count)
        {
            _currentExperiencePerSecond.text = $"{NumberToString.Convert((int)(count * _gameManager.ExperiencePerSecondMultiplier))} {I2.Loc.LocalizationManager.GetTranslation("Inventory/Craft/eps")}";
        }

        private void OnEnable()
        {
            //UpdateExperiencePerSecond(_gameManager.GameState.PlayerState.ExperiencePerSecond);
            UpdateItems();
        }
    }
}