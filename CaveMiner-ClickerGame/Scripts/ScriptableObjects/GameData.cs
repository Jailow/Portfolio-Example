using UnityEngine;
using GD.MinMaxSlider;

namespace CaveMiner
{
    [System.Serializable]
    public class ChestDropData
    {
        [SerializeField] private int _moneyDropCountMin;
        [SerializeField] private int _moneyDropCountMax;
        [SerializeField] private int _dynamiteDropCountMin;
        [SerializeField] private int _dynamiteDropCountMax;
        [SerializeField] private ItemData[] _itemDrops;

        public ItemData[] ItemDrops => _itemDrops;

        public int GetMoneyDropCount()
        {
            return Random.Range(_moneyDropCountMin, _moneyDropCountMax + 1);
        }

        public int GetDynamiteDropCount()
        {
            return Random.Range(_dynamiteDropCountMin, _dynamiteDropCountMax + 1);
        }

        public ItemData GetDropItemData()
        {
            float[] normalizedValue = new float[_itemDrops.Length];
            float valueSum = 0f;

            for (int i = 0; i < _itemDrops.Length; i++)
            {
                normalizedValue[i] = 1f / _itemDrops[i].Value;
                valueSum += normalizedValue[i];
            }

            float value = Random.Range(0f, 1f);
            for(int i = 0; i < _itemDrops.Length; i++)
            {
                if(value > normalizedValue[i] / valueSum)
                {
                    value -= normalizedValue[i] / valueSum;
                }
                else
                {
                    return _itemDrops[i];
                }
            }

            return null;
        }
    }

    [CreateAssetMenu(fileName = "GameData", menuName = "CaveMiner/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Text Colors")]
        [SerializeField] private Color _moneyTextColor;
        [SerializeField] private Color _commonKeyTextColor;
        [SerializeField] private Color _rareKeyTextColor;
        [SerializeField] private Color _epicKeyTextColor;
        [SerializeField] private Color _mythicalKeyTextColor;
        [SerializeField] private Color _legendaryKeyTextColor;
        [Header("Pickaxe")]
        [SerializeField] Sprite[] _pickaxeSprites;
        [SerializeField] private float _basePickaxeSpeedMultiplier;
        [SerializeField] private float _pickaxeMultiplierSpeedPerLevel;
        [SerializeField] private float _experienceMultiplierPerExpLevel;
        [SerializeField] private float _baseChanceToDropMoney;
        [SerializeField] private float _moneyChanceDropMultiplierPerWealthLevel;
        [SerializeField] private int _baseDroppedMoneyCount;
        [SerializeField] private float _baseChanceToMoreMoneyCountDropped;
        [SerializeField] private float _moreMoneyCountDroppedChanceMultiplierPerWealthLevel;
        [SerializeField] private int _autoClickUpgradeMaxClickPerMinute;
        [SerializeField] private AnimationCurve _autoClickPerMinuteCurve;
        [SerializeField] private float _criticalDamageChance;
        [SerializeField] private float _criticalDamageMultiplier;
        [Header("Offline")]
        [SerializeField] private float _defaultMaxOfflineTime;
        [SerializeField] private float _defaultOfflineIncomeMultiplier;
        [Header("Key Drop")]
        [SerializeField] private float _baseChanceToDropCommonKey;
        [SerializeField] private float _baseChanceToDropRareKey;
        [SerializeField] private float _baseChanceToDropEpicKey;
        [SerializeField] private float _baseChanceToDropMythicalKey;
        [SerializeField] private float _baseChanceToDropLegendaryKey;
        [Header("Cave")]
        [SerializeField] private int _baseCaveLevelExperience;
        [SerializeField] private float _caveLevelMulptiplier;
        [SerializeField] private int _priceEveryDepth;
        [SerializeField] private float _caveDifficultyMultiplierPerTenDepth;
        [Header("Rebirth")]
        [SerializeField] private int _maxRebirthLevel;
        [SerializeField] private int _baseExperienceToRebirth;
        [SerializeField] private float _experienceToRebirthMultiplierPerLevel;
        [Header("Shop")]
        [SerializeField] private int _priceToUpdateGameShop;
        [SerializeField] private int _autoUpdateGameShopTime;
        [SerializeField] private float _sellItemPriceMultiplier;
        [Header("Runes")]
        [SerializeField] private int _maxRuneChanceInShop;
        [SerializeField] private int _minRuneChanceInChest;
        [SerializeField] private int _maxRuneChanceInChest;
        [Header("Hammer")]
        [SerializeField] private int _hammerMinChanceInChest;
        [SerializeField] private int _hammerMaxChanceInChest;
        [SerializeField] private int _hammerMaxChanceInShop;
        [Header("Dynamite")]
        [SerializeField] private int _dynamiteBreakBlocksCount;
        [Header("Recycle")]
        [SerializeField] private float _mineralsPerRecycle;
        [SerializeField] private float _experiencePerRecycle;
        [Header("Storage")]
        [SerializeField] private int _maxStorageLevel;
        [SerializeField] private int _defaultMaxExperiencePerSecond;
        [SerializeField] private float _multiplierMaxExperiencePerSecondPerLevel;
        [SerializeField] private int _storageUpgradeDefaultPrice;
        [SerializeField] private float _storageUpgradePriceMultiplierPerLevel;
        [Header("Chests")]
        [SerializeField] private ChestDropData _commonChestDrops;
        [SerializeField] private ChestDropData _rareChestDrops;
        [SerializeField] private ChestDropData _epicChestDrops;
        [SerializeField] private ChestDropData _mythicalChestDrops;
        [SerializeField] private ChestDropData _legendaryChestDrops;

        public ChestDropData CommonChestDrops => _commonChestDrops;
        public ChestDropData RareChestDrops => _rareChestDrops;
        public ChestDropData EpicChestDrops => _epicChestDrops;
        public ChestDropData MythicalChestDrops => _mythicalChestDrops;
        public ChestDropData LegendaryChestDrops => _legendaryChestDrops;

        public Sprite[] PickaxeSprites => _pickaxeSprites;

        // Cave
        public int PriceEveryDepth => _priceEveryDepth;
        public float CaveDifficultyMultiplierPerTenDepth => _caveDifficultyMultiplierPerTenDepth;

        // Storage
        public int MaxStorageLevel => _maxStorageLevel;
        public int DefaultMaxExperiencePerSecond => _defaultMaxExperiencePerSecond;
        public float MultiplierMaxExperiencePerSecondPerLevel => _multiplierMaxExperiencePerSecondPerLevel;
        public int StorageUpgradeDefaultPrice => _storageUpgradeDefaultPrice;
        public float StorageUpgradePriceMultiplierPerLevel => _storageUpgradePriceMultiplierPerLevel;

        // Recycle
        public float MineralsPerRecycle => _mineralsPerRecycle;
        public float ExperiencePerRecycle => _experiencePerRecycle;

        // Offline
        public float DefaultMaxOfflineTime => _defaultMaxOfflineTime;
        public float DefaultOfflineIncomeMultiplier => _defaultOfflineIncomeMultiplier;

        // Dynamite
        public int DynamiteBreakBlocksCount => _dynamiteBreakBlocksCount;

        // Hammer
        public int HammerMaxChanceInShop => _hammerMaxChanceInShop;
        public int HammerMinChanceInChest => _hammerMinChanceInChest;
        public int HammerMaxChanceInChest => _hammerMaxChanceInChest;

        // Runes
        public int MaxRuneChanceInShop => _maxRuneChanceInShop;
        public int MinRuneChanceInChest => _minRuneChanceInChest;
        public int MaxRuneChanceInChest => _maxRuneChanceInChest;

        // Auto Click
        public int AutoClickUpgradeMaxClickPerMinute => _autoClickUpgradeMaxClickPerMinute;
        public AnimationCurve AutoClickPerMinuteCurve => _autoClickPerMinuteCurve;

        // Experience
        public float ExperienceMultiplierPerExpLevel => _experienceMultiplierPerExpLevel;

        // Shop
        public int PriceToUpdateGameShop => _priceToUpdateGameShop;
        public int AutoUpdateGameShopTime => _autoUpdateGameShopTime;
        public float SellItemPriceMultiplier => _sellItemPriceMultiplier;

        // Rebirth
        public int MaxRebirthLevel => _maxRebirthLevel;
        public int BaseExperienceToRebirth => _baseExperienceToRebirth;

        // Critical damage
        public float CriticalDamageChance => _criticalDamageChance;
        public float CriticalDamageMultiplier => _criticalDamageMultiplier;

        // Pickaxe Speed
        public float BasePickaxeSpeedMultiplier => _basePickaxeSpeedMultiplier;
        public float PickaxeMultiplierSpeedPerLevel => _pickaxeMultiplierSpeedPerLevel;

        // Money Drop
        public float BaseChanceToDropMoney => _baseChanceToDropMoney;
        public float MoneyChanceDropMultiplierPerWealthLevel => _moneyChanceDropMultiplierPerWealthLevel;
        public int BaseDroppedMoneyCount => _baseDroppedMoneyCount;
        public float BaseChanceToMoreMoneyCountDropped => _baseChanceToMoreMoneyCountDropped;
        public float MoreMoneyCountDroppedChanceMultiplierPerWealthLevel => _moreMoneyCountDroppedChanceMultiplierPerWealthLevel;

        // Key Drop
        public float BaseChanceToDropCommonKey => _baseChanceToDropCommonKey;
        public float BaseChanceToDropRareKey => _baseChanceToDropRareKey;
        public float BaseChanceToDropEpicKey => _baseChanceToDropEpicKey;
        public float BaseChanceToDropMythicalKey => _baseChanceToDropMythicalKey;
        public float BaseChanceToDropLegendaryKey => _baseChanceToDropLegendaryKey;

        public string MoneyTextColor => ColorUtility.ToHtmlStringRGB(_moneyTextColor);

        public ChestDropData GetChestDropData(RarityType chestType)
        {
            switch (chestType)
            {
                case RarityType.Common:
                    return _commonChestDrops;
                case RarityType.Rare:
                    return _rareChestDrops;
                case RarityType.Epic:
                    return _epicChestDrops;
                case RarityType.Mythical:
                    return _mythicalChestDrops;
                case RarityType.Legendary:
                    return _legendaryChestDrops;
            }

            return null;
        }

        public ItemData[] GetAllDropItemsInChest(RarityType chestType)
        {
            switch (chestType)
            {
                case RarityType.Common:
                    return _commonChestDrops.ItemDrops;
                case RarityType.Rare:
                    return _rareChestDrops.ItemDrops;
                case RarityType.Epic:
                    return _epicChestDrops.ItemDrops;
                case RarityType.Mythical:
                    return _mythicalChestDrops.ItemDrops;
                case RarityType.Legendary:
                    return _legendaryChestDrops.ItemDrops;
            }

            return null;
        }

        public float AllKeyDropChances() 
        {
            return _baseChanceToDropCommonKey + _baseChanceToDropRareKey + _baseChanceToDropEpicKey + _baseChanceToDropMythicalKey + _baseChanceToDropLegendaryKey;
        }

        public string KeyTextColor(RarityType rarityType)
        {
            switch (rarityType)
            {
                case RarityType.Common:
                    return ColorUtility.ToHtmlStringRGB(_commonKeyTextColor);
                case RarityType.Rare:
                    return ColorUtility.ToHtmlStringRGB(_rareKeyTextColor);
                case RarityType.Epic:
                    return ColorUtility.ToHtmlStringRGB(_epicKeyTextColor);
                case RarityType.Mythical:
                    return ColorUtility.ToHtmlStringRGB(_mythicalKeyTextColor);
                case RarityType.Legendary:
                    return ColorUtility.ToHtmlStringRGB(_legendaryKeyTextColor);
            }

            return string.Empty;
        }

        public int GetExperienceCountToNextLevel(int caveLevel)
        {
            return (int)(_baseCaveLevelExperience * Mathf.Pow(_caveLevelMulptiplier, caveLevel - 1));
        }
        
        public int GetExperienceCountToRebirth(int rebirthCount)
        {
            if (rebirthCount <= 0)
                return _baseExperienceToRebirth;

            return (int)(_baseExperienceToRebirth * (rebirthCount + 1) * Mathf.Pow(_experienceToRebirthMultiplierPerLevel, rebirthCount + 1));
        }
    }
}