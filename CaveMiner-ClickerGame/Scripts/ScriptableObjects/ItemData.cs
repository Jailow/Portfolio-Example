using UnityEngine;

namespace CaveMiner
{
    public enum ItemType
    {
        Booster,
        Rune,
        Mineral,
        Hammer,
        Dynamite,
        Resource,
    }

    public enum FilterType
    {
        Scrolls,
        Pickaxe,
        Consumables,
    }

    [CreateAssetMenu(fileName = "Item", menuName = "CaveMiner/Items/Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private ItemType _itemType;
        [SerializeField] private FilterType _filterType;
        [SerializeField] private string _id;
        [SerializeField] private float _value;
        [SerializeField] private bool _haveDescription;
        [SerializeField] private bool _canUse;
        [SerializeField] private bool _canRemove;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _shopCountMin;
        [SerializeField] private int _shopCountMax;
        [SerializeField] private int _shopPriceMin;
        [SerializeField] private int _shopPriceMax;
        [SerializeField] private float _iconSize;

        public float IconSize => _iconSize;
        public FilterType FilterType => _filterType;
        public ItemType ItemType => _itemType;
        public string Id => _id;
        public float Value => _value;
        public bool HaveDescription => _haveDescription;
        public bool CanUse => _canUse;
        public bool CanRemove => _canRemove;
        public Sprite Icon => _icon;
        public int ShopPriceMin => _shopPriceMin;
        public int ShopPriceMax => _shopPriceMax;
        public int ShopCountMin => _shopCountMin;
        public int ShopCountMax => _shopCountMax;
    }
}