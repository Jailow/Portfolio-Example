using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public class UISelectCountPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Slider _slider;

        public bool IsOpened { get; private set; }

        private GameManager _gameManager;
        private UIManager _uiManager;
        private Action<int> _onSelected;
        private int _minCount;
        private int _maxCount;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _closeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Hide();
            });

            _backgroundCloseButton.onClick.AddListener(Hide);

            _acceptButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();

                int selectedCount = (int)Mathf.Lerp(_minCount, _maxCount, _slider.value);
                _onSelected?.Invoke(selectedCount);

                Hide();
            });

            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            int selectedCount = (int)Mathf.Lerp(_minCount, _maxCount, _slider.value);
            _countText.text = $"{NumberToString.Convert(selectedCount)}/{NumberToString.Convert(_maxCount)}";
        }

        public void Show(int minCount, int maxCount, Action<int> onSelected)
        {
            gameObject.SetActive(true);

            _onSelected = onSelected;
            _minCount = minCount;
            _maxCount = maxCount;

            _slider.value = 0f;
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IsOpened = true;
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}