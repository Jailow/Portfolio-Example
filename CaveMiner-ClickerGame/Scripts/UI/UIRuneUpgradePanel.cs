using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CaveMiner.UI
{
    public class UIRuneUpgradePanel : MonoBehaviour
    {
        [SerializeField] private Image _additionalSlotImg;
        [SerializeField] private Sprite _defaultSlotSprite;
        [SerializeField] private Sprite _runeSlotSprite;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Image _currentRuneIcon;
        [SerializeField] private Button _additionalSlotButton;
        [SerializeField] private Image _additionalSlotRuneIcon;
        [SerializeField] private GameObject _plusObj;
        [SerializeField] private TextMeshProUGUI _percentText;
        [SerializeField] private UIRuneLevelRoller _runeLevelRoller;

        private UIDisabledButtonAlpha _connectAlphaButton;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private PickaxeRuneState _currentRuneState;
        private ItemState _currentSelectedItem;

        public UIRuneLevelRoller RuneLevelRoller => _runeLevelRoller;
        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _connectAlphaButton = _connectButton.GetComponent<UIDisabledButtonAlpha>();
            _connectAlphaButton.Init();

            _runeLevelRoller.Init();

            _additionalSlotButton.onClick.AddListener(() => uiManager.InventorySelectionPanel.Show(OnItemTake, new ItemType[1] { ItemType.Hammer }, new string[1] { _currentRuneState.Id }));
            _connectButton.onClick.AddListener(Connect);
            _closeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Hide();
            });
            _backgroundCloseButton.onClick.AddListener(Hide);
        }

        private void OnItemTake(ItemState itemState)
        {
            var item = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (item == null)
                return;

            _currentSelectedItem = itemState;

            _percentText.text = itemState.CustomValue;
            _additionalSlotRuneIcon.sprite = item.Icon;
            _additionalSlotRuneIcon.gameObject.SetActive(true);
            _plusObj.gameObject.SetActive(false);

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            switch (itemData.ItemType)
            {
                case ItemType.Rune:
                    _additionalSlotImg.sprite = _runeSlotSprite;

                    var runeItem = itemData as RuneItemData;
                    if (runeItem != null && _currentRuneState.Level >= runeItem.MaxLevel)
                    {
                        _connectAlphaButton.Interactable = false;
                    }
                    else
                    {
                        _connectAlphaButton.Interactable = true;
                    }
                    break;
                case ItemType.Hammer:
                    _additionalSlotImg.sprite = _defaultSlotSprite;
                    _connectAlphaButton.Interactable = true;
                    break;
            }
        }

        private void Connect()
        {
            _uiManager.ButtonClickSound();

            var playerState = _gameManager.GameState.PlayerState;

            _currentSelectedItem.Count--;
            if(_currentSelectedItem.Count <= 0)
            {
                playerState.Items.Remove(_currentSelectedItem);
            }

            _currentRuneIcon.gameObject.SetActive(false);
            _additionalSlotRuneIcon.gameObject.SetActive(false);
            _plusObj.gameObject.SetActive(false);

            bool isBroken = false;

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == _currentSelectedItem.Id);

            Dictionary<string, object> properties = new Dictionary<string, object>();

            switch (itemData.ItemType)
            {
                case ItemType.Rune:
                    isBroken = Random.Range(0, 100) > int.Parse(_currentSelectedItem.CustomValue.ToString().TrimEnd('%'));

                    #region UpgradeRuneEvent
                    properties.Add("CaveLevel", playerState.CaveLevel);
                    properties.Add("RebirthCount", playerState.Stats.RebirthCount);
                    properties.Add("PlaceChance", _currentSelectedItem.CustomValue);
                    properties.Add("CurrentRuneLevel", _currentRuneState.Level);
                    properties.Add("RuneType", (itemData as RuneItemData).RuneType);
                    properties.Add("IsBroken", isBroken);

                    AmplitudeManager.Instance.Event(AnalyticEventKey.UPGRADE_RUNE, properties);
                    #endregion

                    break;
                case ItemType.Hammer:
                    isBroken = Random.Range(0, 100) <= int.Parse(_currentSelectedItem.CustomValue.ToString().TrimEnd('%'));

                    var currentRuneItemData = _gameManager.Items.FirstOrDefault(e => e.Id == _currentRuneState.Id) as RuneItemData;

                    #region DestroyRuneEvent
                    properties.Add("CaveLevel", playerState.CaveLevel);
                    properties.Add("RebirthCount", playerState.Stats.RebirthCount);
                    properties.Add("PlaceChance", _currentSelectedItem.CustomValue);
                    properties.Add("CurrentRuneLevel", _currentRuneState.Level);
                    properties.Add("RuneType", currentRuneItemData.RuneType);
                    properties.Add("IsBroken", isBroken);

                    AmplitudeManager.Instance.Event(AnalyticEventKey.DESTROY_RUNE, properties);
                    #endregion
                    break;
            }

            _uiManager.UpgradeRuneAnimation.Show(_currentRuneIcon.sprite, itemData.Icon, _currentRuneState.Level, itemData.ItemType, isBroken, OnAnimationCompleted);

            if (isBroken)
            {
                playerState.Stats.BreakRuneCount++;

                AchievementManager.Instance.CheckBreakRuneCountCondition(playerState.Stats.BreakRuneCount);

                switch (itemData.ItemType)
                {
                    case ItemType.Rune:
                        _currentRuneState.Level--;
                        break;
                    case ItemType.Hammer:
                        _currentRuneState.Level = 0;
                        break;
                }

                if (_currentRuneState.Level <= 0)
                {
                    _currentRuneState.Id = string.Empty;
                    _currentRuneState.Level = 0;
                }
            }
            else
            {
                switch (itemData.ItemType)
                {
                    case ItemType.Rune:
                        _currentRuneState.Level++;
                        break;
                }

                AchievementManager.Instance.CheckMaxRuneLevelCondition(_gameManager, playerState.PickaxeState);
            }

            _currentSelectedItem = null;
            _connectAlphaButton.Interactable = false;

            _gameManager.SaveGame();
        }

        private void OnAnimationCompleted()
        {
            _percentText.text = "0%";
            _plusObj.gameObject.SetActive(true);

            if (_currentRuneState.Level <= 0)
            {
                _uiManager.UpgradesScreen.RunesPanel.UpdateSlots();
                Hide();
            }
            else
            {
                _currentRuneIcon.gameObject.SetActive(true);
                _additionalSlotImg.sprite = _runeSlotSprite;
            }
        }

        public void Show(int slotIndex)
        {
            var playerState = _gameManager.GameState.PlayerState;
            _currentRuneState = playerState.PickaxeState.Runes[slotIndex];
            var runeItem = _gameManager.Items.FirstOrDefault(e => e.Id == _currentRuneState.Id) as RuneItemData;

            if (runeItem == null)
                return;

            _currentRuneIcon.gameObject.SetActive(true);
            _currentRuneIcon.sprite = runeItem.Icon;

            _additionalSlotImg.sprite = _runeSlotSprite;

            _percentText.text = "0%";
            _additionalSlotRuneIcon.gameObject.SetActive(false);
            _plusObj.gameObject.SetActive(true);
            _connectAlphaButton.Interactable = false;

            gameObject.SetActive(true);

            _runeLevelRoller.Set(runeItem.MaxLevel, _currentRuneState.Level);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
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