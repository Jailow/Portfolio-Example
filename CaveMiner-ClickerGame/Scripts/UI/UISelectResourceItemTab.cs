using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UISelectResourceItemTab : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _count;

        public ResourceItemState ItemState { get; private set; }

        private int _oldCount;
        private Action<ResourceItemState, int, Vector3> _onTake;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private Button _btn;

        public void Init(GameManager gameManager, UIManager uiManager, Action<ResourceItemState, int, Vector3> onTake)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _onTake = onTake;

            _btn = GetComponent<Button>();

            _btn.onClick.AddListener(() =>
            {
                uiManager.ShowSelectCountPanel(1, ItemState.Count, OnSelectCount);
            });
        }

        private void OnSelectCount(int count)
        {
            _onTake?.Invoke(ItemState, count, transform.position);
        }
        
        public void Set(ResourceItemState itemState)
        {
            ItemState = itemState;

            _icon.sprite = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == itemState.Id).Icon;
        }

        private void Update()
        {
            if (_oldCount != ItemState.Count)
            {
                _oldCount = ItemState.Count;
                _count.text = ItemState.Count.ToString();
            }
        }
    }
}