using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace CaveMiner.UI
{
    public class UIResourceItemTab : UIFolderItemBase
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _count;

        public ResourceItemState ItemState { get; private set; }

        private int _oldCount;
        private GameManager _gameManager;

        public void Init(GameManager gameManager, ResourceItemState itemState)
        {
            _gameManager = gameManager;
            ItemState = itemState;

            _icon.sprite = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == itemState.Id).Icon;
        }

        private void Update()
        {
            if (!_folderTab.IsOpened)
                return;

            if (_oldCount != ItemState.Count)
            {
                _oldCount = ItemState.Count;
                _count.text = ItemState.Count.ToString();
            }
        }
    }
}