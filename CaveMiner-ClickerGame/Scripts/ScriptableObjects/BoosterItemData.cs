using UnityEngine;

namespace CaveMiner
{
    public enum BoosterType
    {
        Money,
        Experience,
        Combo,
    }

    [CreateAssetMenu(fileName = "Booster Item", menuName = "CaveMiner/Items/Booster")]
    public class BoosterItemData : ItemData
    {
        [SerializeField] private BoosterType _boosterType;
        [SerializeField] private float _activeTime;
        [SerializeField] private float _experienceMultiplier;
        [SerializeField] private float _moneyMultiplier;

        public BoosterType BoosterType => _boosterType;
        public float ActiveTime => _activeTime;
        public float ExperienceMultiplier => _experienceMultiplier;
        public float MoneyMultiplier => _moneyMultiplier;
    }
}