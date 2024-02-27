using CaveMiner.Helpers;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIInventoryItemTab : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private GameObject _fade;

        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _takeButton;
        [SerializeField] private Button _infoButton;

        private ItemState _itemState;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private Action<ItemState> _onItemTake;

        private const string DELETE_ITEM_KEY = "delete_selected_item";
        private const string HAVE_ACTIVATED_BOOSTER_KEY = "have_activated_booster";
        private const string DELETE_KEY = "delete";
        private const string CANCEL_KEY = "cancel";
        private const string CLOSE_KEY = "close";

        public bool FadeActive
        {
            set
            {
                _fade.gameObject.SetActive(value);
            }
        }

        public void Init(GameManager gameManager, UIManager uiManager, Action<ItemState> onItemTake)
        {
            _onItemTake = onItemTake;
            _uiManager = uiManager;
            _gameManager = gameManager;

            _useButton.onClick.AddListener(Use);
            _infoButton.onClick.AddListener(ShowDescription);
            _takeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                onItemTake?.Invoke(_itemState);
            });
            _deleteButton.onClick.AddListener(() => 
            {
                _uiManager.ButtonClickSound();
                uiManager.InfoPanel.Show(DELETE_ITEM_KEY, CANCEL_KEY, DELETE_KEY, null, OnDelete, new string[0]);
            });
        }

        private void ShowDescription()
        {
            if (_itemState == null)
                return;

            _uiManager.InfoPanel.Show($"Description/{_itemState.Id}", string.Empty, CLOSE_KEY, null, null, new string[0]);
        }

        private void Use()
        {
            if (_itemState == null || _itemState.Count <= 0)
                return;

            var item = _gameManager.Items.FirstOrDefault(e => e.Id == _itemState.Id);

            if (item == null)
                return;

            _uiManager.ButtonClickSound();

            switch (item.ItemType)
            {
                case ItemType.Booster:
                    var playerState = _gameManager.GameState.PlayerState;
                    playerState.Stats.UseBoostersCount++;

                    AchievementManager.Instance.CheckUseBoostersCountCondition(playerState.Stats.UseBoostersCount);

                    var useBooster = item as BoosterItemData;

                    foreach(var activeBooster in playerState.BoosterStates)
                    {
                        var booster = _gameManager.Items.FirstOrDefault(e => e.Id == activeBooster.Id) as BoosterItemData;

                        if (booster == null)
                            continue;

                        if((booster.MoneyMultiplier > 0 && useBooster.MoneyMultiplier > 0) || (booster.ExperienceMultiplier > 0 && useBooster.ExperienceMultiplier > 0)) // уже есть активированный бустер данного типа
                        {
                            _uiManager.InfoPanel.Show(HAVE_ACTIVATED_BOOSTER_KEY, string.Empty, CLOSE_KEY, null, null, new string[0]);
                            return;
                        }
                    }

                    DeleteItem(1);
                    _gameManager.AddActiveBooster(item.Id);
                    break;
                case ItemType.Rune:
                    break;
            }
        }

        private void OnDelete()
        {
            var playerState = _gameManager.GameState.PlayerState;
            playerState.Stats.DeleteItemsCount += _itemState.Count;
            playerState.Items.Remove(_itemState);

            gameObject.SetActive(false);

            AchievementManager.Instance.CheckDeleteItemsCount(playerState.Stats.DeleteItemsCount);
        }

        private void DeleteItem(int count)
        {
            _itemState.Count -= count;
            if (_itemState.Count <= 0)
            {
                _gameManager.GameState.PlayerState.Items.Remove(_itemState);
                gameObject.SetActive(false);
            }
            else
            {
                _countText.text = _itemState.Count.ToString();
            }
        }

        public void SetItem(ItemState itemState)
        {
            _itemState = itemState;

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (itemData == null)
                return;

            _itemIcon.sprite = itemData.Icon;
            _itemIcon.rectTransform.sizeDelta = CachedVector2.One * itemData.IconSize;

            _countText.text = itemState.Count.ToString();
            _nameText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{_itemState.Id}"), _itemState.CustomValue);

            _deleteButton.gameObject.SetActive(itemData.CanRemove);
            _useButton.gameObject.SetActive(itemData.CanUse);
            _takeButton.gameObject.SetActive(false);
            _infoButton.gameObject.SetActive(itemData.HaveDescription);
        }

        public void SetSelectionItem(ItemState itemState)
        {
            _itemState = itemState;

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (itemData == null)
                return;

            _itemIcon.sprite = itemData.Icon;
            _itemIcon.rectTransform.sizeDelta = CachedVector2.One * itemData.IconSize;

            _countText.text = itemState.Count.ToString();
            _nameText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{itemState.Id}"), itemState.CustomValue);

            _deleteButton.gameObject.SetActive(false);
            _useButton.gameObject.SetActive(false);
            _takeButton.gameObject.SetActive(true);
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