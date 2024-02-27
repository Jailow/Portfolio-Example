using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

namespace CaveMiner.UI
{
    public class UIRebirthUpgradeTab : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private Image _icon;
        [SerializeField] private RebirthUpgrade _rebirthUpgrade;
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private Color _defaultLevelTextColor;
        [SerializeField] private Color _maxLevelTextColor;
        [SerializeField] private UIRebirthUpgradeLine _rebirthUpgradeLine;

        private GameManager _gameManager;
        private Color _color;

        public void Init(GameManager gameManager, Action<RebirthUpgrade> onSelect)
        {
            _gameManager = gameManager;

            _btn.onClick.AddListener(() => onSelect?.Invoke(_rebirthUpgrade));

            _color = new Color(1f, 1f, 1f);

            UpdateStats();
        }

        private void SetAlpha(float alpha)
        {
            _color.a = alpha;
            _icon.color = _color;
            _btn.image.color = _color;

            Color textColor = _currentLevelText.color;
            textColor.a = alpha;
            _currentLevelText.color = textColor;
        }

        private void UpdateStats()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var rebirthUpgradeState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == _rebirthUpgrade.Id);

            if(rebirthUpgradeState == null)
            {
                _currentLevelText.text = "0";
                return;
            }

            _currentLevelText.text = rebirthUpgradeState.Level.ToString();
            _currentLevelText.color = rebirthUpgradeState.Level >= _rebirthUpgrade.MaxLevel ? _maxLevelTextColor : _defaultLevelTextColor;

            CheckUnlockedVisual();
        }

        private void CheckUnlockedVisual()
        {
            bool isUnlocked = _rebirthUpgrade.CheckCondition(_gameManager);
            SetAlpha(isUnlocked ? 1f : 0.5f);
            _rebirthUpgradeLine?.SetState(isUnlocked);
        }

        private void OnAddedRebirthUpgradeLevel(string upgradeId)
        {
            if (upgradeId != _rebirthUpgrade.Id)
            {
                CheckUnlockedVisual();
                return;
            }

            UpdateStats();
        }

        private void OnEnable()
        {
            _gameManager.onAddedRebirthUpgradeLevel += OnAddedRebirthUpgradeLevel;

            UpdateStats();
        }

        private void OnDisable()
        {
            _gameManager.onAddedRebirthUpgradeLevel += OnAddedRebirthUpgradeLevel;
        }
    }
}