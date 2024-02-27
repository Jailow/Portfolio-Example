using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GraphicsQuality
{
    Low,
    High,
}

namespace CaveMiner.UI
{
    public class UIGraphicsSelectionTab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;

        private GameManager _gameManager;
        private UILanguageSelectionTab _languageSelectionTab;

        private int _graphicsQualityCount;

        public void Init(GameManager gameManager, UILanguageSelectionTab languageSelectionTab)
        {
            _gameManager = gameManager;
            _languageSelectionTab = languageSelectionTab;

            _graphicsQualityCount = Enum.GetValues(typeof(GraphicsQuality)).Length;

            _nextButton.onClick.AddListener(() =>
            {
                int currentIndex = (int)_gameManager.GameState.GraphicsQuality + 1;

                if (currentIndex >= _graphicsQualityCount)
                    currentIndex = 0;

                _gameManager.SetGraphicsQuality((GraphicsQuality)currentIndex);
                UpdateVisual();
            });

            _prevButton.onClick.AddListener(() =>
            {
                int currentIndex = (int)_gameManager.GameState.GraphicsQuality - 1;

                if (currentIndex < 0)
                    currentIndex = _graphicsQualityCount - 1;

                _gameManager.SetGraphicsQuality((GraphicsQuality)currentIndex);
                UpdateVisual();
            });
        }

        private void UpdateVisual()
        {
            switch (_gameManager.GameState.GraphicsQuality)
            {
                case GraphicsQuality.Low:
                    _title.text = I2.Loc.LocalizationManager.GetTranslation("Settings/graphics_low");
                    break;
                case GraphicsQuality.High:
                    _title.text = I2.Loc.LocalizationManager.GetTranslation("Settings/graphics_high");
                    break;
            }
        }

        private void OnEnable()
        {
            _languageSelectionTab.onLanguageChanged += UpdateVisual;

            UpdateVisual();
        }

        private void OnDisable()
        {
            _languageSelectionTab.onLanguageChanged -= UpdateVisual;
        }
    }
}