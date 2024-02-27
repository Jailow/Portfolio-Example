using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using I2.Loc;

namespace CaveMiner.UI
{
    public class UITaskTab : UIFolderItemBase
    {
        [SerializeField] private TextMeshProUGUI _progressBarText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private Button _takeRewardBtn;
        [SerializeField] private Image _fill;
        [SerializeField] private Sprite _yellowFillSprite;
        [SerializeField] private Sprite _greenFillSprite;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private TaskData _taskData;

        public TaskState TaskState { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager, Action onTakeReward)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _takeRewardBtn.onClick.AddListener(() => 
            {
                TakeReward();
                onTakeReward?.Invoke();
            });
        }

        public void Set(TaskState taskState)
        {
            TaskState = taskState;

            _taskData = _gameManager.Tasks.FirstOrDefault(e => e.Id == taskState.Id);

            _rewardText.text = taskState.RewardValue.ToString();
            _rewardIcon.sprite = _taskData.RewardIcon;

            UpdateVisual();
        }

        private void Update()
        {
            if (!_folderTab.IsOpened)
                return;

            UpdateProgressBar();
        }

        private void UpdateVisual()
        {
            _nameText.text = _uiManager.GetTaskName(_taskData, TaskState);

            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            float value = (float)TaskState.CurrentValue / (float)TaskState.TargetValue;

            _takeRewardBtn.interactable = value >= 1f;
            _fill.fillAmount = Mathf.Clamp01(value);
            _fill.sprite = value >= 1f ? _greenFillSprite : _yellowFillSprite;
            _progressBarText.text = value >= 1f ? I2.Loc.LocalizationManager.GetTranslation("Tasks/take_reward") : $"{TaskState.CurrentValue}/{TaskState.TargetValue}";
        }

        private void TakeReward()
        {
            var playerState = _gameManager.GameState.PlayerState;
            playerState.Stats.CompletedTasks++;

            AchievementManager.Instance.CheckCompleteTasksCountCondition(playerState.Stats.CompletedTasks);

            playerState.Tasks.Remove(TaskState);

            switch (_taskData.RewardType)
            {
                case TaskRewardType.Experience:
                    _gameManager.AddExperience(TaskState.RewardValue);
                    break;
                case TaskRewardType.Minerals:
                    _gameManager.AddMoney(TaskState.RewardValue);
                    break;
                case TaskRewardType.CommonKey:
                    _gameManager.AddKey(RarityType.Common, TaskState.RewardValue);
                    break;
                case TaskRewardType.RareKey:
                    _gameManager.AddKey(RarityType.Rare, TaskState.RewardValue);
                    break;
                case TaskRewardType.EpicKey:
                    _gameManager.AddKey(RarityType.Epic, TaskState.RewardValue);
                    break;
                case TaskRewardType.MythicalKey:
                    _gameManager.AddKey(RarityType.Mythical, TaskState.RewardValue);
                    break;
                case TaskRewardType.LegendaryKey:
                    _gameManager.AddKey(RarityType.Legendary, TaskState.RewardValue);
                    break;
            }

            _taskData = null;
            TaskState = null;
        }
    }
}