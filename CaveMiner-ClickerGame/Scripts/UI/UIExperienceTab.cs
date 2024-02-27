using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public class UIExperienceTab : MonoBehaviour
    {
        [SerializeField] private Image _fillImg;
        [SerializeField] private TextMeshProUGUI _countText;

        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _gameManager.onExperienceChanged += OnExperienceChanged;

            UpdateTab();
        }

        private void OnExperienceChanged(int count)
        {
            int needCount = _gameManager.GetExperienceCountToNextLevel();
            _fillImg.fillAmount = (float)count / needCount;
            _countText.text = $"{NumberToString.Convert(count)}/{NumberToString.Convert(needCount)}";
        }

        private void UpdateTab()
        {
            var playerState = _gameManager.GameState.PlayerState;
            OnExperienceChanged(playerState.Experience);
        }

        private void OnEnable()
        {
            if (_gameManager != null)
            {
                UpdateTab();
            }
        }
    }
}