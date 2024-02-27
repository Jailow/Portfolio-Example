using UnityEngine;

namespace CaveMiner.UI
{
    public class UIShopScreen : MonoBehaviour
    {
        [SerializeField] private UIShopCategoryButton[] _categoryButtons;
        [SerializeField] private UIGameShopPanel _gameShopPanel;
        [SerializeField] private UISellItemsPanel _sellItemsPanel;

        private UICategoryButton _currentCategoryButton;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameShopPanel.Init(gameManager, uiManager);
            _sellItemsPanel.Init(gameManager, uiManager);

            foreach (var btn in _categoryButtons)
            {
                btn.Init(uiManager, SelectCategory);
            }

            SelectCategory(_categoryButtons[0]);
        }

        public void SelectCategory(int index)
        {
            SelectCategory(_categoryButtons[index]);
        }

        public void SelectCategory(UICategoryButton button)
        {
            if (_currentCategoryButton == button)
                return;

            _currentCategoryButton?.Deselect();
            _currentCategoryButton = button;
            button.Select();
        }
    }
}