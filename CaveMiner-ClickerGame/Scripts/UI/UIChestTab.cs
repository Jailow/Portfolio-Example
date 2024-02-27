using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CaveMiner.UI
{
    public class UIChestTab : MonoBehaviour
    {
        [SerializeField] private RarityType _chestRarityType;
        [SerializeField] private Sprite _chestIcon;
        [SerializeField] private TextMeshProUGUI _keyCount;
        [SerializeField] private Button _openBtn;

        private UIDisabledButtonAlpha _disabledAlphaButton;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private bool _moneyDropped;
        private ItemState _lastDropItem;
        private int dropCount;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _disabledAlphaButton = _openBtn.GetComponent<UIDisabledButtonAlpha>();
            _disabledAlphaButton.Init();

            _openBtn.onClick.AddListener(OpenChest);
        }

        private void OpenChest()
        {
            if (GetKeyCount() <= 0)
                return;

            _uiManager.ButtonClickSound();

            var playerState = _gameManager.GameState.PlayerState;

            var dropItem = _gameManager.GameData.GetChestDropData(_chestRarityType).GetDropItemData();

            #region OpenChestEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("OpenedChest", _chestRarityType);
            properties.Add("DroppedItem", dropItem.Id);
            #endregion

            playerState.Stats.ChestOpenedCount++;

            AchievementManager.Instance.CheckOpenedChestsCountCondition(playerState.Stats.ChestOpenedCount);

            _gameManager.OnChestOpened(_chestRarityType);
            _gameManager.AddKey(_chestRarityType, -1);

            string customValue = string.Empty;

            switch (dropItem.ItemType)
            {
                case ItemType.Rune:
                    customValue = $"{Random.Range(_gameManager.GameData.MinRuneChanceInChest, _gameManager.GameData.MaxRuneChanceInChest + 1)}%";
                    break;
                case ItemType.Hammer:
                    customValue = $"{Random.Range(_gameManager.GameData.HammerMinChanceInChest, _gameManager.GameData.HammerMaxChanceInChest + 1)}%";
                    break;
            }

            properties.Add("DropCustomValue", customValue);

            _moneyDropped = dropItem.ItemType == ItemType.Mineral;

            switch (dropItem.ItemType)
            {
                case ItemType.Mineral:
                    dropCount = _gameManager.GameData.GetChestDropData(_chestRarityType).GetMoneyDropCount();
                    _gameManager.AddMoney(dropCount);
                    break;
                case ItemType.Dynamite:
                    dropCount = _gameManager.GameData.GetChestDropData(_chestRarityType).GetDynamiteDropCount();
                    _lastDropItem = _gameManager.AddItem(dropItem.Id, dropCount, customValue);
                    break;
                default:
                    dropCount = 1;
                    _lastDropItem = _gameManager.AddItem(dropItem.Id, dropCount, customValue);
                    break;
            }

            if (_gameManager.GameState.QuickOpeningIsOn)
            {
                if (_moneyDropped)
                {
                    _uiManager.ChestDropFlyingTexts.Show($"<color=#{_gameManager.GameData.MoneyTextColor}>+{Helpers.NumberToString.Convert(dropCount)} MINERALS");
                }
                else
                {
                    _uiManager.ChestDropFlyingTexts.Show($"+{dropCount} {string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{dropItem.Id}"), customValue)}");
                }
            }
            else
            {
                _uiManager.OpenChestAnimation.Show(_chestIcon, _gameManager.GameData.GetAllDropItemsInChest(_chestRarityType), dropItem, OnAnimationCompleted);
            }

            _gameManager.SaveGame();

            AmplitudeManager.Instance.Event(AnalyticEventKey.OPEN_CHEST, properties);
        }

        private void OnAnimationCompleted()
        {
            if (_moneyDropped)
            {
                _uiManager.ChestDropFlyingTexts.Show($"<color=#{_gameManager.GameData.MoneyTextColor}>+{Helpers.NumberToString.Convert(dropCount)} MINERALS");
            }
            else
            {
                _uiManager.ChestDropFlyingTexts.Show($"+{dropCount} {string.Format(I2.Loc.LocalizationManager.GetTranslation($"Items/{_lastDropItem.Id}"), _lastDropItem.CustomValue)}");
            }
        }

        private void UpdateVisual()
        {
            int keyCount = GetKeyCount();

            _keyCount.text = keyCount.ToString();

            _disabledAlphaButton.Interactable = keyCount > 0;
        }

        private int GetKeyCount()
        {
            var playerState = _gameManager.GameState.PlayerState;
            int keyCount = 0;

            switch (_chestRarityType)
            {
                case RarityType.Common:
                    keyCount = playerState.CommonKeyCount;
                    break;
                case RarityType.Rare:
                    keyCount = playerState.RareKeyCount;
                    break;
                case RarityType.Epic:
                    keyCount = playerState.EpicKeyCount;
                    break;
                case RarityType.Mythical:
                    keyCount = playerState.MythicalKeyCount;
                    break;
                case RarityType.Legendary:
                    keyCount = playerState.LegendaryKeyCount;
                    break;
            }

            return keyCount;
        }

        private void OnEnable()
        {
            UpdateVisual();

            _gameManager.onKeyChanged += UpdateVisual;
        }

        private void OnDisable()
        {
            _gameManager.onKeyChanged -= UpdateVisual;
        }
    }
}