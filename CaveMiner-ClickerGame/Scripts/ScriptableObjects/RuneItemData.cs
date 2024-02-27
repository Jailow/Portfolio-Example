using UnityEngine;

namespace CaveMiner
{
    public enum RuneType
    {
        Fortune, // шанс выбить деньги
        KeyKeeper, // шанс выбить ключ
        Impulse, // шанс сломать несколько блоков подряд
        Treasury, // шанс появление дорогого блока
        Sharpening, // шанс получить доп опыт с блока
    }

    [CreateAssetMenu(fileName = "Rune Item", menuName = "CaveMiner/Items/Rune")]
    public class RuneItemData : ItemData
    {
        [SerializeField] private RuneType _runeType;
        [SerializeField] private float _firstDefaultValue;
        [SerializeField] private float _firstMultiplierPerLevel;
        [SerializeField] private float _secondDefaultValue;
        [SerializeField] private float _secondMultiplierPerLevel;
        [SerializeField] private int _maxLevel;

        public RuneType RuneType => _runeType;
        public float FirstDefaultValue => _firstDefaultValue;
        public float FirstMultiplierPerLevel => _firstMultiplierPerLevel;
        public float SecondDefaultValue => _secondDefaultValue;
        public float SecondMultiplierPerLevel => _secondMultiplierPerLevel;
        public int MaxLevel => _maxLevel;

        public float GetFirstValue(int level)
        {
            return _firstDefaultValue + ((level - 1) * _firstMultiplierPerLevel);
        }

        public float GetSecondValue(int level)
        {
            return _secondDefaultValue + ((level - 1) * _secondMultiplierPerLevel);
        }
    }
}