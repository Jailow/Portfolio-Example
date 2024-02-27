using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CaveMiner.Helpers;
using System.Linq;
using System;

namespace CaveMiner.UI
{
    public class UICraftItemTab : MonoBehaviour
    {
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Button _btn;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _resourceCount;
        [SerializeField] private TextMeshProUGUI _experiencePerSecond;
        [SerializeField] private Color _defaultTextColor;
        [SerializeField] private Color _redTextColor;

        private GameManager _gameManager;
        private CraftItemData _craftItemData;
        private UIDisabledButtonAlpha _disabledButton;

        public void Init(GameManager gameManager, UIManager uiManager, Action<UICraftItemTab, CraftItemData> onCraft)
        {
            _gameManager = gameManager;

            _btn.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                onCraft?.Invoke(this, _craftItemData);
            });

            _disabledButton = _btn.GetComponent<UIDisabledButtonAlpha>();
            _disabledButton.Init();
        }

        public void Set(CraftItemData craftItemData)
        {
            _craftItemData = craftItemData;

            _itemIcon.sprite = craftItemData.Icon;
            _resourceIcon.sprite = craftItemData.ResourceToCraft.Icon;

            _name.text = I2.Loc.LocalizationManager.GetTranslation($"Inventory/Craft/{craftItemData.Id}");
            _experiencePerSecond.text = $"+{NumberToString.Convert(_craftItemData.ExperiencePerSecond)} {I2.Loc.LocalizationManager.GetTranslation("Inventory/Craft/eps")}";

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_craftItemData == null)
                return;

            var playerState = _gameManager.GameState.PlayerState;
            var itemState = playerState.ResourceItems.FirstOrDefault(e => e.Id == _craftItemData.ResourceToCraft.Id);

            int count = itemState == null ? 0 : itemState.Count;

            bool isUnlocked = count >= _craftItemData.CountToCraft;

            _resourceCount.text = $"{count}/{_craftItemData.CountToCraft}";
            _resourceCount.color = isUnlocked ? _defaultTextColor : _redTextColor;
            _disabledButton.Interactable = isUnlocked;
        }

        private void OnResourceItemAdded(ResourceItemState resourceItemState)
        {
            if(resourceItemState.Id == _craftItemData.ResourceToCraft.Id)
                UpdateVisual();
        }

        private void OnEnable()
        {
            _gameManager.onResourceItemAdded += OnResourceItemAdded;

            UpdateVisual();
        }

        private void OnDisable()
        {
            _gameManager.onResourceItemAdded -= OnResourceItemAdded;
        }
    }
}