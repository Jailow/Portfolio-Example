using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public enum InventoryCategoryType
    {
        Backpack,
        Resources,
        Recycling,
    }

    public class UIInventoryScreen : MonoBehaviour
    {
        [SerializeField] private UIInventoryCategoryButton[] _categoryButtons;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private UIBackpackPanel _backpackPanel;
        [SerializeField] private UIResourcesPanel _resourcesPanel;
        [SerializeField] private UIRecyclingPanel _recyclingPanel;

        private UICategoryButton _currentCategoryButton;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _recyclingPanel.Init(gameManager, uiManager);
            _backpackPanel.Init(gameManager, uiManager);
            _resourcesPanel.Init(gameManager, uiManager);

            foreach(var btn in _categoryButtons)
            {
                btn.Init(uiManager, SelectCategory);
            }

            SelectCategory(_categoryButtons[0]);
        }

        public void SelectCategory(UICategoryButton button)
        {
            if (_currentCategoryButton == button)
                return;

            _currentCategoryButton?.Deselect();
            _currentCategoryButton = button;
            button.Select();

            UpdateTitle();
        }
        
        private void UpdateTitle()
        {
            var categoryButton = _currentCategoryButton as UIInventoryCategoryButton;
            _title.text = I2.Loc.LocalizationManager.GetTranslation($"Inventory/{categoryButton.CategoryType.ToString().ToLowerInvariant()}");
        }

        private void OnEnable()
        {
            UpdateTitle();
        }
    }
}