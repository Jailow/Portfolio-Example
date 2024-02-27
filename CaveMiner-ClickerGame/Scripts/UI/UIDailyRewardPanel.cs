using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIDailyRewardPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;

        public bool IsOpened { get; private set; }

        private UIDailyRewardItem[] _rewardItems;
        private GameManager _gameManager;
        private UIManager _uiManager;

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _uiManager = FindObjectOfType<UIManager>();

            _closeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Hide();
            });

            _rewardItems = GetComponentsInChildren<UIDailyRewardItem>(true);
            foreach (var item in _rewardItems)
                item.Init(_uiManager, OnTakeReward);

            _backgroundCloseButton.onClick.AddListener(Hide);
        }

        public void OnTakeReward(UIDailyRewardItem rewardItem)
        {
            _closeButton.gameObject.SetActive(true);
            _backgroundCloseButton.interactable = true;

            rewardItem.SetSlotState(DailyRewardState.Taken);

            var playerState = _gameManager.GameState.PlayerState;
            playerState.DailyRewardStrike++;
            playerState.LastDailyRewardTimestamp = (int)ServerTimeManager.Instance.ServerTime;

            switch (rewardItem.RewardType)
            {
                case DailyRewardType.Minerals:
                    _gameManager.AddMoney(int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.Experience:
                    _gameManager.AddExperience(int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.CommonKey:
                    _gameManager.AddKey(RarityType.Common, int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.RareKey:
                    _gameManager.AddKey(RarityType.Rare, int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.EpicKey:
                    _gameManager.AddKey(RarityType.Epic, int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.MythicalKey:
                    _gameManager.AddKey(RarityType.Mythical, int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.LegendaryKey:
                    _gameManager.AddKey(RarityType.Legendary, int.Parse(rewardItem.Value));
                    break;
                case DailyRewardType.Item:
                    string[] values = rewardItem.Value.Split(';');
                    var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == values[0]);

                    if (itemData == null)
                        itemData = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == values[0]);

                    if (values.Length > 2)
                    {
                        _gameManager.AddItem(itemData.Id, int.Parse(values[1]), values[2]);
                    }
                    else
                    {
                        _gameManager.AddItem(itemData.Id, int.Parse(values[1]), string.Empty);
                    }
                    break;
            }

            AchievementManager.Instance.CheckDailyRewardStrikeCondition(playerState.DailyRewardStrike);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            _closeButton.gameObject.SetActive(false);
            _backgroundCloseButton.interactable = false;

            var playerState = _gameManager.GameState.PlayerState;

            for (int i = 0; i < _rewardItems.Length; i++)
            {
                if (playerState.DailyRewardStrike == i || (playerState.DailyRewardStrike > _rewardItems.Length - 1 && i >= _rewardItems.Length - 1)) // Если текущий день или последний бесконечный
                {
                    _rewardItems[i].SetSlotState(DailyRewardState.Active);
                    if (i > 0)
                        _rewardItems[i - 1].LineActive(true);
                }
                else if (playerState.DailyRewardStrike > i) // Если уже прошедшие, взятые награды
                {
                    _rewardItems[i].SetSlotState(DailyRewardState.Taken);

                    if (i > 0)
                        _rewardItems[i - 1].LineActive(true);
                }
                else // Следующие награды
                {
                    _rewardItems[i].SetSlotState(DailyRewardState.Closed);

                    if (i > 0)
                        _rewardItems[i - 1].LineActive(false);
                }
            }
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