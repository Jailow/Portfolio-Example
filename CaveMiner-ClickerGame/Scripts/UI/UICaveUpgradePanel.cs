using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public class UICaveUpgradePanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _upgradeBtn;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _averagePriceText;
        [SerializeField] private GameObject _bottomObj;
        [SerializeField] private GameObject _maxLevelObj;
        [SerializeField] private UINeedItemTab _mineralsItemTab;
        [SerializeField] private UINeedItemTab _experienceItemTab;
        [SerializeField] private UINeedItemTab[] _resourceItemTabs;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private CaveData _caveData;
        private UIDisabledButtonAlpha _disabledButton;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _upgradeBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Upgrade();
            });

            _disabledButton = _upgradeBtn.GetComponent<UIDisabledButtonAlpha>();
            _disabledButton.Init();

            _closeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Hide();
            });

            _backgroundCloseButton.onClick.AddListener(Hide);
        }

        private void Upgrade()
        {
            var caveState = _gameManager.GetCaveState(_caveData.Id);

            var caveLevelData = _caveData.GetLevelData(caveState.Level);
            bool needMinerals = caveLevelData.MineralsCountToUnlock > 0;
            bool needExperience = caveLevelData.ExperienceCountToUnlock > 0;

            _gameManager.UpgradeCaveLevel(_caveData.Id);

            if (needMinerals)
                _gameManager.AddMoney(-caveLevelData.MineralsCountToUnlock);

            if (needExperience)
                _gameManager.AddExperience(-caveLevelData.ExperienceCountToUnlock);

            for (int i = 0; i < caveLevelData.ItemsToUnlock.Length; i++)
            {
                var itemData = caveLevelData.ItemsToUnlock[i].ItemData;

                if (itemData.ItemType == ItemType.Resource)
                {
                    _gameManager.AddResourceItem(itemData.Id, -caveLevelData.ItemsToUnlock[i].Count);
                }
                else
                {
                    _gameManager.RemoveItemFromInventory(itemData.Id, -caveLevelData.ItemsToUnlock[i].Count);
                }
            }

            AchievementManager.Instance.CheckCaveLevelCondition(_gameManager.GameState.PlayerState);

            UpdateVisual();
        }

        public void Show(CaveData caveData)
        {
            gameObject.SetActive(true);

            _caveData = caveData;

            _icon.sprite = caveData.Icon;
            _nameText.text = I2.Loc.LocalizationManager.GetTranslation($"CaveSelection/Cave/{caveData.Id}");
            UpdateVisual();

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void UpdateVisual()
        {
            gameObject.SetActive(true);

            var caveState = _gameManager.GetCaveState(_caveData.Id);

            _averagePriceText.text = _caveData.GetAveragePricePerBlock(caveState.Level).ToString("0.##");
            _levelText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"CaveSelection/level"), caveState.Level);

            bool isMaxLevel = caveState.Level >= _caveData.MaxLevel;
            _bottomObj.SetActive(!isMaxLevel);
            _maxLevelObj.SetActive(isMaxLevel);

            UpdateItems();

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void UpdateItems()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var caveState = _gameManager.GetCaveState(_caveData.Id);

            var caveLevelData = _caveData.GetLevelData(caveState.Level);
            bool needMinerals = caveLevelData.MineralsCountToUnlock > 0;
            bool needExperience = caveLevelData.ExperienceCountToUnlock > 0;
            bool haveAllItems = true;

            _mineralsItemTab.gameObject.SetActive(needMinerals);
            _experienceItemTab.gameObject.SetActive(needExperience);

            if (needMinerals)
            {
                _mineralsItemTab.Text.text = $"{NumberToString.Convert(playerState.Money)}/{NumberToString.Convert(caveLevelData.MineralsCountToUnlock)}";
                _mineralsItemTab.IsNotEnough = playerState.Money < caveLevelData.MineralsCountToUnlock;

                if (haveAllItems && playerState.Money < caveLevelData.MineralsCountToUnlock)
                    haveAllItems = false;
            }

            if (needExperience)
            {
                _experienceItemTab.Text.text = $"{NumberToString.Convert(playerState.Experience)}/{NumberToString.Convert(caveLevelData.ExperienceCountToUnlock)}";
                _experienceItemTab.IsNotEnough = playerState.Experience < caveLevelData.ExperienceCountToUnlock;

                if (haveAllItems && playerState.Experience < caveLevelData.ExperienceCountToUnlock)
                    haveAllItems = false;
            }

            foreach (var item in _resourceItemTabs)
                item.gameObject.SetActive(false);

            for (int i = 0; i < caveLevelData.ItemsToUnlock.Length; i++)
            {
                _resourceItemTabs[i].gameObject.SetActive(true);

                var itemData = caveLevelData.ItemsToUnlock[i].ItemData;

                _resourceItemTabs[i].Icon.sprite = itemData.Icon;

                int itemCount = 0;

                if (itemData.ItemType == ItemType.Resource)
                {
                    var resourceItemState = playerState.ResourceItems.FirstOrDefault(e => e.Id == itemData.Id);

                    if (resourceItemState != null)
                        itemCount = resourceItemState.Count;
                }
                else
                {
                    var itemState = playerState.Items.FirstOrDefault(e => e.Id == itemData.Id);

                    if (itemState == null)
                        itemCount = itemState.Count;
                }

                _resourceItemTabs[i].Text.text = $"{NumberToString.Convert(itemCount)}/{NumberToString.Convert(caveLevelData.ItemsToUnlock[i].Count)}";
                _resourceItemTabs[i].IsNotEnough = itemCount < caveLevelData.ItemsToUnlock[i].Count;

                if (haveAllItems && itemCount < caveLevelData.ItemsToUnlock[i].Count)
                    haveAllItems = false;
            }

            _disabledButton.Interactable = haveAllItems;
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