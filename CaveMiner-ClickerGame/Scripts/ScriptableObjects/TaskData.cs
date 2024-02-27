using UnityEngine;

namespace CaveMiner
{
    public enum TaskCategory
    {
        Cave,
    }
    
    public enum TaskType
    {
        DestroyBlocks,
        DropKeys,
        OpenChests,
        ClickCount,
    }

    public enum TaskRewardType
    {
        Experience,
        Minerals,
        CommonKey,
        RareKey,
        EpicKey,
        MythicalKey,
        LegendaryKey,
    }

    [CreateAssetMenu(fileName = "New Task", menuName = "CaveMiner/Task")]
    public class TaskData : ScriptableObject
    {
        [SerializeField] private TaskCategory _taskCategory;
        [SerializeField] private string _id;
        [Header("Target")]
        [SerializeField] private TaskType _taskType;
        [SerializeField] private int _minTargetValue;
        [SerializeField] private int _maxTargetValue;
        [Header("Reward")]
        [SerializeField] private TaskRewardType _rewardType;
        [SerializeField] private Sprite _rewardIcon;
        [SerializeField] private int _minReward;
        [SerializeField] private int _maxReward;

        public TaskCategory TaskCategory => _taskCategory;
        public TaskRewardType RewardType => _rewardType;
        public TaskType TaskType => _taskType;
        public Sprite RewardIcon => _rewardIcon;
        public string Id => _id;

        public int TargetValue => Random.Range(_minTargetValue, _maxTargetValue + 1);
        public int Reward => Random.Range(_minReward, _maxReward + 1);
    }
}