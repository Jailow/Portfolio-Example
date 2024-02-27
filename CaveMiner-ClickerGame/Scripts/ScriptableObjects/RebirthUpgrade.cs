using System.Linq;
using UnityEngine;

namespace CaveMiner
{
    public enum RebirthConditionType
    {
        None,
        UnlockedCave,
        RebirthUpgradeLevel,
        PickaxeDamageLevel,
        PickaxeWealthLevel,
        PickaxeExperienceLevel,
        PickaxeAutoClickLevel,
        RebirthCount,
    }

    public enum RebirthUpgradeType
    {
        DropKeyChance,
        DropMineralsChance,
    }

    [System.Serializable]
    public class RebirthUpgradeCondition
    {
        [SerializeField] private RebirthConditionType _conditionType;
        [SerializeField] private string _value;

        public RebirthConditionType ConditionType => _conditionType;
        public string Value => _value;
    }

    [CreateAssetMenu(fileName = "Rebirth Upgrade", menuName = "CaveMiner/RebirthUpgrade")]
    public class RebirthUpgrade : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _value;
        [SerializeField] private int _maxLevel;
        [SerializeField] private string _firstSymbol;
        [SerializeField] private string _secondSymbol;
        [SerializeField] private RebirthUpgradeCondition _condition;

        public string Id => _id;
        public Sprite Icon => _icon;
        public float Value => _value;
        public int MaxLevel => _maxLevel;
        public string FirstSymbol => _firstSymbol;
        public string SecondSymbol => _secondSymbol;
        public RebirthUpgradeCondition Condition => _condition;

        public bool CheckCondition(GameManager gameManager)
        {
            if (_condition == null || _condition.ConditionType == RebirthConditionType.None)
            {
                return true;
            }

            var playerState = gameManager.GameState.PlayerState;

            int intValue = 0;
            string stringValue = string.Empty;


            switch (_condition.ConditionType)
            {
                case RebirthConditionType.UnlockedCave:
                    if(!playerState.CaveStates.Any(e => e.Id == _condition.Value && e.Level >= 1))
                        return false;
                    break;
                case RebirthConditionType.RebirthUpgradeLevel:
                    string[] rebirthUpgradeLevelValues = _condition.Value.Split(';');
                    intValue = int.Parse(rebirthUpgradeLevelValues[1]);

                    var rebirthUpgrade = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == rebirthUpgradeLevelValues[0]);
                    if (rebirthUpgrade != null)
                    {
                        if (rebirthUpgrade.Level < intValue)
                            return false;
                    }
                    break;
                case RebirthConditionType.PickaxeDamageLevel:
                    intValue = int.Parse(_condition.Value);
                    if (playerState.PickaxeState.DamageLevel + 1 < intValue)
                        return false;
                    break;
                case RebirthConditionType.PickaxeWealthLevel:
                    intValue = int.Parse(_condition.Value);
                    if (playerState.PickaxeState.WealthLevel + 1 < intValue)
                        return false;
                    break;
                case RebirthConditionType.PickaxeExperienceLevel:
                    intValue = int.Parse(_condition.Value);
                    if (playerState.PickaxeState.ExperienceLevel + 1 < intValue)
                        return false;
                    break;
                case RebirthConditionType.PickaxeAutoClickLevel:
                    intValue = int.Parse(_condition.Value);
                    if (playerState.PickaxeState.AutoClickLevel < intValue)
                        return false;
                    break;
                case RebirthConditionType.RebirthCount:
                    intValue = int.Parse(_condition.Value);
                    if (playerState.Stats.RebirthCount < intValue)
                        return false;
                    break;
            }

            return true;
        }
    }
}