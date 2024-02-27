using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Numerics;

namespace CaveMiner.UI
{
    public class UIRebirthScreen : MonoBehaviour
    {
        [SerializeField] private Button _rebirthBtn;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private AnimationCurve _scrollRectShowCurve;
        [SerializeField] private UIRebirthExperienceTab _experienceTab;
        [SerializeField] private TextMeshProUGUI _rebirthPointsText;
        //[SerializeField] private Transform _upgradeParent;
        //[SerializeField] private UIRebirthUpgradeTab _upgradeTabPrefab;
        [SerializeField] private UIRebirthCategoryButton[] _categoryButtons;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private UIRebirthCategoryButton _selectedCategoryButton;
        private UIDisabledButtonAlpha _rebirthDisabledButton;

        private const string REBIRTH_INFO_KEY = "rebirth";
        private const string YES_KEY = "yes";
        private const string NO_KEY = "no";

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _gameManager.onRebirthPointsChanged += UpdatePointsCount;
            _gameManager.onExperienceChanged += OnExperienceChanged;

            _rebirthDisabledButton = _rebirthBtn.GetComponent<UIDisabledButtonAlpha>();
            _rebirthDisabledButton.Init();

            _rebirthBtn.onClick.AddListener(Rebirth);

            _experienceTab.Init(gameManager);

            var rebirthUpgrades = GetComponentsInChildren<UIRebirthUpgradeTab>(true);
            foreach (var rebirthUpgrade in rebirthUpgrades)
            {
                rebirthUpgrade.Init(gameManager, OnSelectRebirthUpgrade);
            }

            foreach(var categoryButton in _categoryButtons)
            {
                categoryButton.Init(uiManager, OnSelectCategory);
            }

            OnSelectCategory(_categoryButtons[0]);

            UpdateStats();
        }

        private void OnSelectRebirthUpgrade(RebirthUpgrade rebirthUpgrade)
        {
            _uiManager.RebirthUpgradePanel.Show(rebirthUpgrade);
        }

        private void OnSelectCategory(UIRebirthCategoryButton button)
        {
            _selectedCategoryButton?.Deselect();
            _selectedCategoryButton = button;
            button.Select();
        }

        private void Rebirth()
        {
            var playerState = _gameManager.GameState.PlayerState;
            int experienceToRebirth = _gameManager.GetExperienceCountToNextRebirth();

            if (playerState.Experience < experienceToRebirth)
            {
                return;
            }

            _uiManager.ButtonClickSound();
            _uiManager.InfoPanel.Show(REBIRTH_INFO_KEY, YES_KEY, NO_KEY, OnRebirth, null, new string[0]);
        }

        private void OnRebirth()
        {
            var playerState = _gameManager.GameState.PlayerState;
            playerState.Stats.RebirthCount++;

            AchievementManager.Instance.CheckRebirthCountCondition(playerState.Stats.RebirthCount);

            #region RebirthEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("RebirthCount", playerState.Stats.RebirthCount);
            properties.Add("PickaxeSpeed", playerState.PickaxeState.DamageLevel);
            properties.Add("PickaxeWealth", playerState.PickaxeState.WealthLevel);
            properties.Add("PickaxeExperience", playerState.PickaxeState.ExperienceLevel);
            properties.Add("PickaxeAutoClick", playerState.PickaxeState.AutoClickLevel);
            properties.Add("CaveLevel", playerState.CaveLevel);

            AmplitudeManager.Instance.Event(AnalyticEventKey.REBIRTH, properties);
            #endregion

            if (!_gameManager.GameState.NoAds)
                CleverAdsSolutionsManager.Instance.ShowInterstitial();

            _gameManager.AddRebirthPoints(1);
            _gameManager.AddExperience(-playerState.Experience);
            _gameManager.AddMoney(-playerState.Money);
            _gameManager.Pickaxe.AddDamage(-(playerState.PickaxeState.DamageLevel - 1));
            _gameManager.Pickaxe.AddWealth(-(playerState.PickaxeState.WealthLevel - 1));
            _gameManager.Pickaxe.AddExperience(-(playerState.PickaxeState.ExperienceLevel - 1));
            _gameManager.Pickaxe.AddAutoClick(-(playerState.PickaxeState.AutoClickLevel));

            playerState.StorageState.Level = 1;
            playerState.StorageState.ExperiencePerSecond = 0;

            playerState.Tasks.Clear();

            foreach (var resourceItem in playerState.ResourceItems)
                resourceItem.Count = 0;

            playerState.CraftedItemsIds.Clear();
            
            //foreach(var runeState in playerState.PickaxeState.Runes)
            //{
            //    if (string.IsNullOrEmpty(runeState.Id))
            //    {
            //        runeState.Level = 0;
            //    }
            //    else
            //    {
            //        runeState.Level = 1;
            //    }
            //}

            playerState.CaveLevel = 1;
            _gameManager.SelectCave("C001");
            _gameManager.GenerateNewCaveTasks("C001", 3);

            foreach(var booster in playerState.BoosterStates.ToArray())
            {
                _gameManager.RemoveActiveBooster(booster);
            }

            #region RebirthCountEvent
            switch (playerState.Stats.RebirthCount)
            {
                case 1:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_1");
                    break;
                case 2:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_2");
                    break;
                case 3:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_3");
                    break;
                case 4:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_4");
                    break;
                case 5:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_5");
                    break;
                case 10:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_10");
                    break;
                case 25:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_25");
                    break;
                case 50:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_50");
                    break;
                case 100:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Rebirth_100");
                    break;
            }
            #endregion

            _gameManager.ReportCaveLevel(_gameManager.GameState.PlayerState.CaveLevel);
            _gameManager.ReportRebirthCount(_gameManager.GameState.PlayerState.Stats.RebirthCount);

            _gameManager.SaveGame();
        }

        private void UpdatePointsCount(int count)
        {
            _rebirthPointsText.text = count.ToString();
        }

        private void OnExperienceChanged(int count)
        {
            if (_gameManager.GameState.PlayerState.Stats.RebirthCount >= _gameManager.GameData.MaxRebirthLevel)
            {
                _rebirthDisabledButton.Interactable = false;
                return;
            }

            int experienceToRebirth = _gameManager.GetExperienceCountToNextRebirth();
            _rebirthDisabledButton.Interactable = count >= experienceToRebirth;
        }

        private void UpdateStats()
        {
            var playerState = _gameManager.GameState.PlayerState;
            UpdatePointsCount(playerState.RebirthPoints);
            OnExperienceChanged(playerState.Experience);
        }

        private void OnEnable()
        {
            _scrollRect.verticalNormalizedPosition = 1f - (1f / 5) * 3f;
            _scrollRect.DOVerticalNormalizedPos(1f, 1f).SetEase(_scrollRectShowCurve).SetAutoKill(true);

            if (_gameManager != null)
            {
                UpdateStats();
            }
        }
    }
}