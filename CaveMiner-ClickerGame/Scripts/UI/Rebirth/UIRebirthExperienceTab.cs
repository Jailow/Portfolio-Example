using CaveMiner.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRebirthExperienceTab : MonoBehaviour
    {
        [SerializeField] private Image _fillImg;
        [SerializeField] private TextMeshProUGUI _countText;

        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _gameManager.onExperienceChanged += OnExperienceChanged;

            UpdateStats();
        }

        private void OnExperienceChanged(int count)
        {
            if(_gameManager.GameState.PlayerState.Stats.RebirthCount >= _gameManager.GameData.MaxRebirthLevel)
            {
                _fillImg.fillAmount = 1f;
                _countText.text = $"MAX LEVEL";
                return;
            }

            int needCount = _gameManager.GetExperienceCountToNextRebirth();
            _fillImg.fillAmount = (float)count / needCount;
            _countText.text = $"{NumberToString.Convert(count)}/{NumberToString.Convert(needCount)}";
        }

        private void UpdateStats()
        {
            var playerState = _gameManager.GameState.PlayerState;
            OnExperienceChanged(playerState.Experience);
        }

        private void OnEnable()
        {
            if (_gameManager != null)
            {
                UpdateStats();
            }
        }
    }
}