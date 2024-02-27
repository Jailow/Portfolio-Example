using I2.Loc;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LanguageCode
{
    en,
    ru,
    uk,
    fr,
    pt,
    es,
}

namespace CaveMiner.UI
{
    public class UILanguageSelectionTab : MonoBehaviour
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;

        private List<string> _allLanguages;
        private int _currentLanguageIndex;

        public Action onLanguageChanged;

        public void Init()
        {
            _allLanguages = LocalizationManager.GetAllLanguages();

            for(int i = 0; i < _allLanguages.Count; i++)
            {
                if (LocalizationManager.CurrentLanguage == _allLanguages[i])
                {
                    _currentLanguageIndex = i;
                    break;
                }
            }
            _nextButton.onClick.AddListener(() =>
            {
                _currentLanguageIndex++;
                if(_currentLanguageIndex >= _allLanguages.Count)
                    _currentLanguageIndex = 0;

                LocalizationManager.CurrentLanguage = _allLanguages[_currentLanguageIndex];

                onLanguageChanged?.Invoke();
            });

            _prevButton.onClick.AddListener(() =>
            {
                _currentLanguageIndex--;
                if (_currentLanguageIndex < 0)
                    _currentLanguageIndex = _allLanguages.Count - 1;

                LocalizationManager.CurrentLanguage = _allLanguages[_currentLanguageIndex];

                onLanguageChanged?.Invoke();
            });
        }
    }
}