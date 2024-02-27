using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRecyclingSlot : MonoBehaviour
    {
        [SerializeField] private GameObject _plusObj;
        [SerializeField] private Image _slotIcon;
        [SerializeField] private TextMeshProUGUI _countText;

        private Button _btn;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private UIRecyclingPanel _recyclingPanel;
        private Action _onResourceItemChanged;

        public void Init(GameManager gameManager, UIManager uiManager, UIRecyclingPanel recyclingPanel, Action onResourceItemChanged)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;
            _recyclingPanel = recyclingPanel;

            _onResourceItemChanged = onResourceItemChanged;

            _btn = GetComponent<Button>();

            _btn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();

                _uiManager.ResourceSelectionPanel.Show(OnResourceItemTake, true);
            });
        }

        private void Update()
        {
            var recyclingState = _gameManager.GameState.PlayerState.RecyclingState;

            if(recyclingState.ResourceItem != null)
            {
                _countText.text = recyclingState.ResourceItem.Count.ToString();
            }
        }

        public void OnResourceItemTake(ResourceItemState resourceItemState, int count, Vector3 pos)
        {
            var recyclingState = _gameManager.GameState.PlayerState.RecyclingState;

            if(!string.IsNullOrEmpty(recyclingState.ResourceItem.Id) && recyclingState.ResourceItem.Count > 0)
            {
                _gameManager.AddResourceItem(recyclingState.ResourceItem.Id, recyclingState.ResourceItem.Count);
            }

            recyclingState.ResourceItem.Id = resourceItemState.Id;
            recyclingState.ResourceItem.Count = count;

            var resourceItemData = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == resourceItemState.Id);

            float recycleTime = resourceItemData.RecyclingTime;
            if (_gameManager.GameState.SpeedRecycle)
                recycleTime /= 5f;

            recyclingState.FirstPlaceResourceRecycleTime = count * recycleTime;

            _gameManager.AddResourceItem(resourceItemState.Id, -count);

            _uiManager.ResourceSelectionPanel.Hide();

            UpdateVisual();

            _onResourceItemChanged?.Invoke();
        }

        private void OnResourceItemChanged()
        {
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var recyclingState = playerState.RecyclingState;

            bool haveItem = !string.IsNullOrEmpty(recyclingState.ResourceItem.Id);

            _plusObj.SetActive(!haveItem);
            _countText.gameObject.SetActive(haveItem);
            _slotIcon.gameObject.SetActive(haveItem);

            if (haveItem)
            {
                var resourceItemData = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == recyclingState.ResourceItem.Id);
                _slotIcon.sprite = resourceItemData.Icon;
                _countText.text = recyclingState.ResourceItem.Count.ToString();
            }
        }

        private void OnEnable()
        {
            _recyclingPanel.onResourceItemChanged += OnResourceItemChanged;
        }

        private void OnDisable()
        {
            _recyclingPanel.onResourceItemChanged -= OnResourceItemChanged;
        }
    }
}