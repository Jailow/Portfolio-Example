using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using DG.Tweening;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public class UIRecyclingPanel : MonoBehaviour
    {
        [SerializeField] private UIRecyclingSlot _slot;
        [SerializeField] private Image _progressBarFill;
        [SerializeField] private GameObject _mineralsIncomeObj;
        [SerializeField] private TextMeshProUGUI _mineralsIncomeText;
        [SerializeField] private GameObject _experienceIncomeObj;
        [SerializeField] private TextMeshProUGUI _experienceIncomeText;
        [SerializeField] private UIRecycleTypeSwitch _typeSwitch;
        [SerializeField] private TextMeshProUGUI _timeLeftText;
        [SerializeField] private TextMeshProUGUI _watchAdsButtonText;
        [SerializeField] private Button _watchAdsButton;
        [SerializeField] private Button _collectButton;

        private UIDisabledButtonAlpha _collectDisabledButton;
        private ResourceItemData _resourceItemData;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private Tween _tween;

        private const int MAX_WATCH_REWARD_ADS = 5;
        private const float RECYCLE_TIME_TO_SHOW_REWARDED_ADS = 90;
        private const int TIME_TO_RESTART_REWARDED_ADS = 1800;

        public Action onResourceItemChanged;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _timeLeftText.text = string.Empty;

            _collectDisabledButton = _collectButton.GetComponent<UIDisabledButtonAlpha>();
            _collectDisabledButton.Init();

            _typeSwitch.Init(gameManager, uiManager);
            _slot.Init(gameManager, uiManager, this, OnResourceItemChanged);

            _collectButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Collect();
            });

            _watchAdsButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                CleverAdsSolutionsManager.Instance.ShowRewarded(OnRewarded);
            });
        }

        private void Update()
        {
            if(_resourceItemData == null)
            {
                _watchAdsButton.gameObject.SetActive(false);
                return;
            }

            var recycleState = _gameManager.GameState.PlayerState.RecyclingState;

            if(recycleState.FirstPlaceResourceRecycleTime >= RECYCLE_TIME_TO_SHOW_REWARDED_ADS && recycleState.RewardAdsLeft > 0)
            {
                _watchAdsButton.gameObject.SetActive(CleverAdsSolutionsManager.Instance.RewardedAdReady);
            }
            else
            {
                _watchAdsButton.gameObject.SetActive(false);
            }
        }

        private float GetRecycleTime()
        {
            float recycleTime = _resourceItemData.RecyclingTime;

            var playerState = _gameManager.GameState.PlayerState;
            var recycleSpeedMultiplier = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "recycle_speed_multiplier");
            var recycleSpeedMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "recycle_speed_multiplier");

            recycleTime *= 1f - ((recycleSpeedMultiplier.Value * recycleSpeedMultiplierState.Level) * 0.01f);

            if (_gameManager.GameState.SpeedRecycle)
                recycleTime /= 5f;

            return recycleTime;
        }

        private void OnRewarded()
        {
            var recycleState = _gameManager.GameState.PlayerState.RecyclingState;

            bool instantly = false;

            int diff = (int)(ServerTimeManager.Instance.ServerTime - recycleState.LastTimestamp);

            float recycleTime = GetRecycleTime();

            if (recycleState.FirstPlaceResourceRecycleTime <= 600f)
            {
                instantly = true;
            }
            else if (recycleState.FirstPlaceResourceRecycleTime <= 1800)
            {
                if (recycleState.ResourceItem.Count * recycleTime <= 600)
                    instantly = true;
                else
                    recycleState.LastTimestamp -= 600 + diff;
            }
            else if (recycleState.FirstPlaceResourceRecycleTime <= 3600)
            {
                if (recycleState.ResourceItem.Count * recycleTime <= 1800)
                    instantly = true;
                else
                    recycleState.LastTimestamp -= 1800 + diff;
            }
            else
            {
                if (recycleState.ResourceItem.Count * recycleTime <= 3600)
                    instantly = true;
                else
                    recycleState.LastTimestamp -= 3600 + diff;
            }

            if (instantly)
            {
                recycleState.LastTimestamp = 0;
            }

            if (recycleState.RewardAdsLeft == MAX_WATCH_REWARD_ADS)
            {
                recycleState.FirstWatchRewardAdTimestamp = ServerTimeManager.Instance.ServerTime;
            }

            recycleState.RewardAdsLeft--;


            UpdateVisual();

            StopAllCoroutines();

            CalculateOfflineIncome();

            _gameManager.SaveGame();

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("Type", "Skip Recycling");

            AmplitudeManager.Instance.Event(AnalyticEventKey.WATCH_REWARDED_AD, properties);
        }

        private void Collect()
        {
            var recycleState = _gameManager.GameState.PlayerState.RecyclingState;

            if (recycleState.Minerals >= 1f)
            {
                _gameManager.AddMoney((int)recycleState.Minerals);
                recycleState.Minerals = 0;
            }

            if(recycleState.Experience >= 1f)
            {
                _gameManager.AddExperience((int)recycleState.Experience);
                recycleState.Experience = 0;
            }

            recycleState.Minerals = 0;
            recycleState.Experience = 0;

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            var recycleState = _gameManager.GameState.PlayerState.RecyclingState;

            _mineralsIncomeObj.SetActive(recycleState.Minerals >= 1f);
            _experienceIncomeObj.SetActive(recycleState.Experience >= 1f);

            if (recycleState.Minerals >= 1f)
            {
                _mineralsIncomeText.text = NumberToString.Convert((int)recycleState.Minerals);
            }

            if (recycleState.Experience >= 1f)
            {
                _experienceIncomeText.text = NumberToString.Convert((int)recycleState.Experience);
            }

            _collectDisabledButton.Interactable = recycleState.Minerals >= 1f || recycleState.Experience >= 1f;
        }

        private void UpdateWatchAdsButtonText()
        {
            if (_resourceItemData == null)
                return;

            var recycleState = _gameManager.GameState.PlayerState.RecyclingState;

            if(recycleState.FirstPlaceResourceRecycleTime <= 90)
            {
                return;
            }

            bool instantly = false;

            float recycleTime = GetRecycleTime();

            if (recycleState.FirstPlaceResourceRecycleTime <= 600f)
            {
                instantly = true;
            }
            else if (recycleState.FirstPlaceResourceRecycleTime <= 1800)
            {
                if (recycleState.ResourceItem.Count * recycleTime <= 600)
                    instantly = true;
                else
                    _watchAdsButtonText.text = I2.Loc.LocalizationManager.GetTranslation("Inventory/recycle_10_min");
            }
            else if (recycleState.FirstPlaceResourceRecycleTime <= 3600)
            {
                if (recycleState.ResourceItem.Count * recycleTime <= 1800)
                    instantly = true;
                else
                    _watchAdsButtonText.text = I2.Loc.LocalizationManager.GetTranslation("Inventory/recycle_30_min");
            }
            else
            {
                if (recycleState.ResourceItem.Count * recycleTime <= 3600)
                    instantly = true;
                else
                    _watchAdsButtonText.text = I2.Loc.LocalizationManager.GetTranslation("Inventory/recycle_60_min");
            }

            if (instantly)
            {
                _watchAdsButtonText.text = I2.Loc.LocalizationManager.GetTranslation("Inventory/recycle_instantly");
            }
        }

        private IEnumerator RecycleAnimation()
        {
            _tween?.Kill();

            var playerState = _gameManager.GameState.PlayerState;
            var recyclingState = playerState.RecyclingState;

            float recycleTime = GetRecycleTime();

            float diff = (ServerTimeManager.Instance.ServerTime - recyclingState.LastTimestamp);

            float allTimeLeft = recyclingState.ResourceItem.Count * recycleTime - diff;
            _timeLeftText.text = SecondsToTimeString(allTimeLeft);

            _progressBarFill.fillAmount = diff / recycleTime;
            _tween = _progressBarFill.DOFillAmount(1f, recycleTime - diff).SetEase(Ease.Linear).SetAutoKill(true);

            float time = recycleTime - diff;

            while (time > 0)
            {
                time -= Time.deltaTime;

                allTimeLeft -= Time.deltaTime;
                _timeLeftText.text = SecondsToTimeString(allTimeLeft);

                yield return null;
            }

            recyclingState.LastTimestamp = ServerTimeManager.Instance.ServerTime;
            RecycleItem(1);

            while (recyclingState.ResourceItem.Count > 0)
            {
                _tween?.Kill();
                _progressBarFill.fillAmount = 0f;
                _tween = _progressBarFill.DOFillAmount(1f, recycleTime).SetEase(Ease.Linear).SetAutoKill(true);

                time = recycleTime;
                while (time > 0)
                {
                    time -= Time.deltaTime;

                    allTimeLeft -= Time.deltaTime;
                    _timeLeftText.text = SecondsToTimeString(allTimeLeft);

                    yield return null;
                }

                recyclingState.LastTimestamp = ServerTimeManager.Instance.ServerTime;
                RecycleItem(1);
            }
        }

        private string SecondsToTimeString(float allSeconds)
        {
            int hours = Mathf.FloorToInt(allSeconds / 3600);
            int minutes = Mathf.FloorToInt((allSeconds % 3600) / 60);
            int seconds = Mathf.Clamp(Mathf.FloorToInt(allSeconds % 60), 0, int.MaxValue);

            string timeString = "";

            if (hours > 0)
                timeString += hours.ToString("D2") + ":";

            if (minutes > 0 || hours > 0)
                timeString += minutes.ToString("D2") + ":";

            timeString += seconds.ToString("D2");

            return timeString;
        }

        private void RecycleItem(int count)
        {
            var playerState = _gameManager.GameState.PlayerState;
            var recyclingState = playerState.RecyclingState;

            count = Mathf.Clamp(count, 0, recyclingState.ResourceItem.Count);
            recyclingState.ResourceItem.Count -= count;

            playerState.Stats.RecycleItemsCount += count;

            var recycleSpeedMultiplier = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "recycle_income_multiplier");
            var recycleSpeedMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "recycle_income_multiplier");

            float incomeMultiplier = 1f + ((recycleSpeedMultiplier.Value * recycleSpeedMultiplierState.Level) * 0.01f);

            switch (recyclingState.RecycleType)
            {
                case 0:
                    recyclingState.Experience += ((Mathf.Pow(_resourceItemData.RecyclingTime, 2) * _gameManager.GameData.ExperiencePerRecycle) * count) * incomeMultiplier;
                    break;
                case 1:
                    recyclingState.Minerals += ((Mathf.Pow(_resourceItemData.RecyclingTime, 2) * _gameManager.GameData.MineralsPerRecycle) * count) * incomeMultiplier;
                    break;
            }

            if (recyclingState.ResourceItem.Count <= 0) // Кончились предметы
            {
                _resourceItemData = null;

                recyclingState.ResourceItem.Id = string.Empty;
                recyclingState.ResourceItem.Count = 0;
                recyclingState.LastTimestamp = 0;

                _slot.UpdateVisual();

                _tween?.Kill();
                _progressBarFill.fillAmount = 0f;
                StopAllCoroutines();

                _timeLeftText.text = string.Empty;
            }

            UpdateVisual();
        }

        private void OnResourceItemChanged()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var recyclingState = playerState.RecyclingState;

            recyclingState.LastTimestamp = ServerTimeManager.Instance.ServerTime;

            _resourceItemData = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == recyclingState.ResourceItem.Id);
            _progressBarFill.fillAmount = 0f;

            StopAllCoroutines();
            StartCoroutine(RecycleAnimation());

            UpdateWatchAdsButtonText();
        }

        private void CalculateOfflineIncome()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var recyclingState = playerState.RecyclingState;

            if (_resourceItemData != null)
            {
                float recycleTime = GetRecycleTime();

                float offlineTime = ServerTimeManager.Instance.ServerTime - recyclingState.LastTimestamp;
                int recycleCount = (int)(offlineTime / recycleTime);

                float recyclingTime = recycleTime;
                if (recycleCount > 0)
                {
                    RecycleItem(recycleCount);
                    recyclingState.LastTimestamp += (int)(recyclingTime * recycleCount);
                }
            }

            if(recyclingState.ResourceItem.Count > 0)
            {
                StartCoroutine(RecycleAnimation());
            }

            _slot.UpdateVisual();

            UpdateWatchAdsButtonText();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                OnEnable();
            }
        }

        private void OnEnable()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var recyclingState = playerState.RecyclingState;

            if(ServerTimeManager.Instance.ServerTime - recyclingState.FirstWatchRewardAdTimestamp >= TIME_TO_RESTART_REWARDED_ADS)
            {
                recyclingState.RewardAdsLeft = MAX_WATCH_REWARD_ADS;
            }

            if (_resourceItemData == null)
            {
                if (!string.IsNullOrEmpty(recyclingState.ResourceItem.Id) && recyclingState.ResourceItem.Count > 0)
                {
                    _resourceItemData = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == recyclingState.ResourceItem.Id);
                }
                else
                {
                    _tween?.Kill();
                    _progressBarFill.fillAmount = 0f;
                }
            }

            CalculateOfflineIncome();
            UpdateVisual();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}