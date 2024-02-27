using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIItemTab : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private TextMeshProUGUI _nameText;

        protected GameManager _gameManager;
        protected ItemData _itemData;

        public virtual void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public virtual void SetItem(ItemState itemState)
        {
            _itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (_itemData == null)
                return;

            _itemIcon.sprite = _itemData.Icon;

            _countText.text = itemState.Count.ToString();
            _nameText.text = I2.Loc.LocalizationManager.GetTranslation($"Items/{itemState.Id}");
        }
    }
}