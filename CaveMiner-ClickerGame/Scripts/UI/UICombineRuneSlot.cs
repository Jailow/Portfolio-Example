using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UICombineRuneSlot : MonoBehaviour
    {
        [SerializeField] private GameObject _plusIcon;
        [SerializeField] private Image _icon;

        private Button _btn;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private ItemState _itemState;
        private Action _onTakeItem;

        public ItemState ItemState => _itemState;

        public void Init(GameManager gameManager, UIManager uiManager, Action onTakeItem)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _onTakeItem = onTakeItem;

            _btn = GetComponent<Button>();

            _btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _uiManager.InventorySelectionPanel.Show(OnItemTake, new ItemType[1] { ItemType.Rune }, null);
        }

        public void UpdateSlotVisual()
        {
            bool haveRune = _itemState != null;

            _plusIcon.SetActive(!haveRune);
            _icon.gameObject.SetActive(haveRune);

            if (!haveRune)
                return;

            var item = _gameManager.Items.FirstOrDefault(e => e.Id == _itemState.Id);

            if (item == null)
                return;

            _icon.sprite = item.Icon;
        }

        private void OnItemTake(ItemState itemState)
        {
            _itemState = itemState;

            UpdateSlotVisual();

            _onTakeItem?.Invoke();
        }

        public void ResetSlot()
        {
            _itemState = null;

            UpdateSlotVisual();
        }
    }
}