using DG.Tweening;
using I2.Loc;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _showInterstitialSoon;
        [SerializeField] private Image _launchGameFade;
        [SerializeField] private UIShopScreen _shopScreen;
        [SerializeField] private UIInventoryScreen _inventoryScreen;
        [SerializeField] private UICaveScreen _caveScreen;
        [SerializeField] private UIUpgradesScreen _upgradesScreen;
        [SerializeField] private UIRebirthScreen _rebirthScreen;
        [SerializeField] private UINavigationPanel _navigationPanel;
        [SerializeField] private UIExperienceTab _experienceTab;
        [SerializeField] private UIMoneyTab _moneyTab;
        [SerializeField] private FlyingTextController _caveDropFlyingTexts;
        [SerializeField] private FlyingTextController _chestDropFlyingTexts;
        [Header("Popups")]
        [SerializeField] private UIRebirthUpgradePanel _rebirthUpgradePanel;
        [SerializeField] private UISettingsPanel _settingsPanel;
        [SerializeField] private UIInfoPanel _infoPanel;
        [SerializeField] private UICaveSelectionPanel _caveSelectionPanel;
        [SerializeField] private UIChestsPanel _chestsPanel;
        [SerializeField] private UIInventorySelectionPanel _inventorySelectionPanel;
        [SerializeField] private UIResourceSelectionPanel _resourceSelectionPanel;
        [SerializeField] private UIRuneUpgradePanel _runeUpgradePanel;
        [SerializeField] private UIChangeKeyPanel _changeKeyPanel;
        [SerializeField] private UIOfflineIncomePanel _offlineIncomePanel;
        [SerializeField] private UITasksPanel _tasksPanel;
        [SerializeField] private UIGiftPanel _giftPanel;
        [SerializeField] private UICaveUpgradePanel _caveUpgradePanel;
        [SerializeField] private UIReviewPanel _reviewPanel;
        [SerializeField] private UIDailyRewardPanel _dailyRewardPanel;
        [SerializeField] private UICombineRunePanel _combineRunePanel;
        [SerializeField] private UIUpdatesPanel _updatesPanel;
        [SerializeField] private UINewVersionPanel _newVersionPanel;
        [SerializeField] private UISelectCountPanel _selectCountPanel;
        [SerializeField] private UIStoragePanel _storagePanel;
        [Header("Animations")]
        [SerializeField] private PlaceRuneAnimation _placeRunAnimation;
        [SerializeField] private UpgradeRuneAnimation _upgradeRuneAnimation;
        [SerializeField] private OpenChestAnimation _openChestAnimation;
        [SerializeField] private CombineRuneAnimation _combineRuneAnimation;

        private GameObject _currentPanel;
        private GameManager _gameManager;

        public Canvas Canvas => _canvas;
        public UIRebirthUpgradePanel RebirthUpgradePanel => _rebirthUpgradePanel;
        public UISettingsPanel SettingsPanel => _settingsPanel;
        public OpenChestAnimation OpenChestAnimation => _openChestAnimation;
        public UpgradeRuneAnimation UpgradeRuneAnimation => _upgradeRuneAnimation;
        public UIRuneUpgradePanel RuneUpgradePanel => _runeUpgradePanel;
        public PlaceRuneAnimation PlaceRuneAnimation => _placeRunAnimation;
        public CombineRuneAnimation CombineRuneAnimation => _combineRuneAnimation;
        public UIInventorySelectionPanel InventorySelectionPanel => _inventorySelectionPanel;
        public UIResourceSelectionPanel ResourceSelectionPanel => _resourceSelectionPanel;
        public UICaveSelectionPanel CaveSelectionPanel => _caveSelectionPanel;
        public UIInfoPanel InfoPanel => _infoPanel;
        public UICaveScreen CaveScreen => _caveScreen;
        public UIShopScreen ShopScreen => _shopScreen;
        public UIInventoryScreen InventoryScreen => _inventoryScreen;
        public UIUpgradesScreen UpgradesScreen => _upgradesScreen;
        public UIRebirthScreen RebirthScreen => _rebirthScreen;
        public UINavigationPanel NavigationPanel => _navigationPanel;
        public UIExperienceTab ExperienceTab => _experienceTab;
        public FlyingTextController CaveDropFlyingTexts => _caveDropFlyingTexts;
        public FlyingTextController ChestDropFlyingTexts => _chestDropFlyingTexts;

        private CompletedTaskNotificationController _completedTaskNotificationController;
        private LocalizationParamsManager _localizationParamsManager;

        private void Awake()
        {
            _localizationParamsManager = GetComponent<LocalizationParamsManager>();

            CleverAdsSolutionsManager.Instance.onInterstitialShowSoonStarting += () =>
            {
                _showInterstitialSoon.SetActive(true);
            };

            CleverAdsSolutionsManager.Instance.onInterstitialShowSoonEnded += () =>
            {
                _showInterstitialSoon.SetActive(false);
            };

            _launchGameFade.gameObject.SetActive(true);
        }

        public void Init(GameManager gameManager, CaveController caveController)
        {
            _completedTaskNotificationController = FindObjectOfType<CompletedTaskNotificationController>();
            _completedTaskNotificationController.Init(gameManager, this);

            _gameManager = gameManager;

            _caveScreen.Init(gameManager, caveController, this);
            _rebirthScreen.Init(gameManager, this);
            _upgradesScreen.Init(gameManager, this);
            _experienceTab.Init(gameManager);
            _moneyTab.Init(gameManager, this);
            _inventoryScreen.Init(gameManager, this);
            _shopScreen.Init(gameManager, this);
            _infoPanel.Init(this);
            _caveSelectionPanel.Init(gameManager, this);
            _chestsPanel.Init(gameManager, this);
            _inventorySelectionPanel.Init(gameManager, this);
            _resourceSelectionPanel.Init(gameManager, this);
            _runeUpgradePanel.Init(gameManager, this);
            _rebirthUpgradePanel.Init(gameManager, this);
            _changeKeyPanel.Init(gameManager, this);
            _tasksPanel.Init(gameManager, this);
            _giftPanel.Init(gameManager, this);
            _caveUpgradePanel.Init(gameManager, this);
            _reviewPanel.Init(gameManager, this);
            _combineRunePanel.Init(gameManager, this);
            _updatesPanel.Init(gameManager, this);
            _newVersionPanel.Init(this);
            _selectCountPanel.Init(gameManager, this);
            _storagePanel.Init(gameManager, this);

            _openChestAnimation.Init(gameManager);
            _placeRunAnimation.Init(gameManager);
            _upgradeRuneAnimation.Init(gameManager, this);
            _combineRuneAnimation.Init(gameManager);

            _settingsPanel.Init(gameManager, this);

            _navigationPanel.Init(gameManager, this);

            if (_gameManager.GameState.PlayerState.CaveStates.Count >= 2 && _gameManager.GameState.ViewedVersionUpdate != Application.version)
            {
                _updatesPanel.Show();
            }

            Debug.Log("Ver: " + FirebaseManager.Instance.AppVersion);
            if(!string.IsNullOrEmpty(FirebaseManager.Instance.AppVersion) && FirebaseManager.Instance.AppVersion != Application.version)
            {
                _newVersionPanel.Show();
            }
        }

        public void ShowGame()
        {
            DOTween.Sequence().Append(_launchGameFade.DOFade(0f, 1.5f)).AppendCallback(() => _launchGameFade.gameObject.SetActive(false));
        }

        public void ShowPanel(GameObject panel)
        {
            _currentPanel?.SetActive(false);
            _currentPanel = panel;
            panel.SetActive(true);
        }

        public void OpenSettings()
        {
            _settingsPanel.Show();
        }

        public void OpenCaveSelection()
        {
            _caveSelectionPanel.Show();
        }

        public void OpenChestsPanel()
        {
            _chestsPanel.Show();
        }

        public void OpenChangeKeyPanel()
        {
            _changeKeyPanel.Show();
        }

        public void OpenTasksPanel()
        {
            _tasksPanel.Show();
        }
        
        public void OpenGiftPanel()
        {
            _giftPanel.Show();
        }

        public void ShowCaveUpgradePanel(CaveData caveData)
        {
            _caveUpgradePanel.Show(caveData);
        }

        public void ShowStoragePanel()
        {
            _storagePanel.Show();
        }

        public void ShowReviewPanel()
        {
            _reviewPanel.Show();
        }

        public void ShowCombineRunePanel()
        {
            _combineRunePanel.Show();
        }

        public void OpenOfflineIncomePanel(OfflineIncomeState offlineIncomeState)
        {
            _offlineIncomePanel.Show(offlineIncomeState);
        }

        public void ShowDailyRewardPanel()
        {
            _dailyRewardPanel.Show();
        }

        public void ShowSelectCountPanel(int minCount, int maxCount, Action<int> onSelected)
        {
            _selectCountPanel.Show(minCount, maxCount, onSelected);
        }

        public void ButtonClickSound()
        {
            if (_gameManager.GameState.SoundsIsOn)
            {
                var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                audioShot.Play(_gameManager.SoundData.GetButtonClick());
            }
        }

        public bool HaveOverlayPanel()
        {
            return _rebirthUpgradePanel.IsOpened
                || _settingsPanel.IsOpened
                || _infoPanel.IsOpened
                || _caveSelectionPanel.IsOpened
                || _chestsPanel.IsOpened
                || _inventorySelectionPanel.IsOpened
                || _runeUpgradePanel.IsOpened
                || _placeRunAnimation.IsOpened
                || _upgradeRuneAnimation.IsOpened
                || _openChestAnimation.IsOpened
                || _changeKeyPanel.IsOpened
                || _offlineIncomePanel.IsOpened
                || _tasksPanel.IsOpened
                || _giftPanel.IsOpened
                || _caveUpgradePanel.IsOpened
                || _reviewPanel.IsOpened
                || _dailyRewardPanel.IsOpened
                || _updatesPanel.IsOpened
                || _newVersionPanel.IsOpened
                || _selectCountPanel.IsOpened
                || _resourceSelectionPanel.IsOpened
                || _storagePanel.IsOpened;
        }

        public void TaskCompleted(string taskName)
        {
            _completedTaskNotificationController.Show(taskName);
        }

        public string GetTaskName(TaskData taskData, TaskState taskState)
        {
            try
            {
                switch (taskData.TaskType)
                {
                    case TaskType.DestroyBlocks:
                        string blockName = LocalizationManager.GetTranslation($"Blocks/{_gameManager.BlockDatas.FirstOrDefault(e => e.Id == taskState.NeedValue).Id}");
                        return string.Format(LocalizationManager.GetTranslation($"Tasks/Tasks/{taskState.Id}"), $"<color=#0BCF25>{taskState.TargetValue}", $"{blockName}</color>");
                    case TaskType.DropKeys:
                        _localizationParamsManager.SetParameterValue("COUNT", taskState.TargetValue.ToString());
                        string keyName = LocalizationManager.GetTranslation($"RarityType/{taskState.NeedValue}", true, 0, true, true, _localizationParamsManager.gameObject).Replace(taskState.TargetValue.ToString(), string.Empty);
                        return string.Format(LocalizationManager.GetTranslation($"Tasks/Tasks/{taskState.Id}", true, 0, true, true, _localizationParamsManager.gameObject).Replace(taskState.TargetValue.ToString(), "0"), $"<color=#0BCF25>{taskState.TargetValue} {keyName}</color>");
                    case TaskType.OpenChests:
                        _localizationParamsManager.SetParameterValue("COUNT", taskState.TargetValue.ToString());
                        string chestName = LocalizationManager.GetTranslation($"RarityType/{taskState.NeedValue}", true, 0, true, true, _localizationParamsManager.gameObject).Replace(taskState.TargetValue.ToString(), string.Empty);
                        return string.Format(LocalizationManager.GetTranslation($"Tasks/Tasks/{taskState.Id}", true, 0, true, true, _localizationParamsManager.gameObject).Replace(taskState.TargetValue.ToString(), "0"), $"<color=#0BCF25>{taskState.TargetValue} {chestName}</color>");
                    case TaskType.ClickCount:
                        _localizationParamsManager.SetParameterValue("COUNT", taskState.TargetValue.ToString());
                        return string.Format(LocalizationManager.GetTranslation($"Tasks/Tasks/{taskState.Id}", true, 0, true, true, _localizationParamsManager.gameObject).Replace(taskState.TargetValue.ToString(), "0"), $"<color=#0BCF25>{taskState.TargetValue}</color>");
                }
            }
            catch(System.Exception e)
            {
                Firebase.Crashlytics.Crashlytics.LogException(new System.Exception($"{e.Message} : UIManager.GetTaskName : {taskData.TaskType} : {taskState.NeedValue} : {taskState.Id} : {taskState.TargetValue}"));
            }

            return "MISSING NAME";
        }
    }
}