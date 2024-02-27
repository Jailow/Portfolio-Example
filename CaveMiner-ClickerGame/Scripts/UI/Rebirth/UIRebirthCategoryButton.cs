using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRebirthCategoryButton : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Color _activeTextColor;
        [SerializeField] private Color _inactiveTextColor;

        private Button _btn;
        private TextMeshProUGUI _title;
        private bool _isSelected;

        public void Init(UIManager uiManager, Action<UIRebirthCategoryButton> onSelect)
        {
            _btn = GetComponent<Button>();
            _title = GetComponentInChildren<TextMeshProUGUI>();

            _btn.onClick.AddListener(() => 
            {
                if(!_isSelected)
                    uiManager.ButtonClickSound();

                onSelect?.Invoke(this);
            });
        }

        public void Select()
        {
            _isSelected = true;
            _title.color = _activeTextColor;
            _btn.image.sprite = _activeSprite;
            _panel.SetActive(true);
        }

        public void Deselect()
        {
            _isSelected = false;
            _title.color = _inactiveTextColor;
            _btn.image.sprite = _inactiveSprite;
            _panel.SetActive(false);
        }
    }
}