using CaveMiner.Helpers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CaveMiner.UI
{
    public enum GiftType
    {
        Minerals,
        Experience,
        //AutoClick,
        //EpsBoost,
    }

    public class UIGiftPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _takeButton;
        [SerializeField] private Button _declineButton;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private GiftType _giftType;
        private int _rewardCount;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = FindObjectOfType<GameManager>();
            _uiManager = FindObjectOfType<UIManager>();

            _backgroundCloseButton.onClick.AddListener(Hide);
            _closeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Hide();
            });

            _declineButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Hide();
            });

            _takeButton.onClick.AddListener(() => 
            {
                TakeGift();
            });
        }

        private void TakeGift()
        {
            CleverAdsSolutionsManager.Instance.ShowRewarded(OnGiftTaken);
        }

        private void OnGiftTaken()
        {
            switch (_giftType)
            {
                case GiftType.Minerals:
                    _gameManager.AddMoney(_rewardCount);
                    break;
                case GiftType.Experience:
                    _gameManager.AddExperience(_rewardCount);
                    break;
                //case GiftType.AutoClick:
                //    break;
                //case GiftType.EpsBoost:
                //    break;
            }

            Hide();

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("Type", "Take Gift");

            AmplitudeManager.Instance.Event(AnalyticEventKey.WATCH_REWARDED_AD, properties);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            var playerState = _gameManager.GameState.PlayerState;

            _giftType = (GiftType)(Random.Range(0f, Enum.GetNames(typeof(GiftType)).Length));

            switch (_giftType)
            {
                case GiftType.Minerals:
                    _rewardCount = (int)(Random.Range(20, 50) * (Mathf.Pow(playerState.CaveLevel, 2f) * 2f));
                    _title.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Gift/{_giftType}"), NumberToString.Convert(_rewardCount));
                    break;
                case GiftType.Experience:
                    _rewardCount = (int)(Random.Range(50, 100) * (Mathf.Pow(playerState.CaveLevel, 2f) * 2f));
                    _title.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"Gift/{_giftType}"), NumberToString.Convert(_rewardCount));
                    break;
                //case GiftType.AutoClick:
                //case GiftType.EpsBoost:
                //    _title.text = I2.Loc.LocalizationManager.GetTranslation($"Gift/{_giftType}");
                //    break;
            }

            gameObject.SetActive(false);
            gameObject.SetActive(true);
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