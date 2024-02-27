using UnityEngine;

namespace CaveMiner.UI
{
    public class UIInventoryCategoryButton : UICategoryButton
    {
        [SerializeField] private InventoryCategoryType _categoryType;
        [SerializeField] private GameObject _panel;
        public InventoryCategoryType CategoryType => _categoryType;

        public override void Select()
        {
            base.Select();

            _panel.SetActive(true);
        }

        public override void Deselect()
        {
            base.Deselect();

            _panel.SetActive(false);
        }
    }
}