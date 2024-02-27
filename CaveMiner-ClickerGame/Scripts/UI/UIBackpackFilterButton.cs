using System;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIBackpackFilterButton : MonoBehaviour
    {
        [SerializeField] private FilterType _filterType;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;

        private Button _btn;

        public FilterType FilterType => _filterType;
        public bool IsSelected { get; private set; }

        public void Init(UIManager uiManager, Action onClick)
        {
            _btn = GetComponent<Button>();

            _btn.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                OnClick();

                onClick?.Invoke();
            });

            OnClick();
        }

        private void OnClick()
        {
            IsSelected = !IsSelected;
            _btn.image.sprite = IsSelected ? _activeSprite : _inactiveSprite;
        }
    }
}