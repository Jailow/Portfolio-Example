using UnityEngine;

namespace CaveMiner.UI
{
    public class UIShopCategoryButton : UICategoryButton
    {
        [SerializeField] private GameObject _panel;

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