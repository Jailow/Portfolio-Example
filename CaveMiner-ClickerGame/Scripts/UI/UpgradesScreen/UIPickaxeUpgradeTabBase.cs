using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UIPickaxeUpgradeTabBase : MonoBehaviour
    {
        [SerializeField] protected int _basePrice;
        [SerializeField] protected float _priceMultiplierPerLevel;
        [SerializeField] protected float _maxLevel;
        [SerializeField] protected TextMeshProUGUI _priceText;
        [SerializeField] protected TextMeshProUGUI _levelText;
        [SerializeField] private Button _upgradeBtn;

        protected UIDisabledButtonAlpha _disabledButtonAlpha;
        protected GameManager _gameManager;
        protected UIManager _uiManager;
        protected int _currentPrice;

        protected const string MAX_LEVEL_KEY = "Upgrades/max";

        public virtual void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _disabledButtonAlpha = _upgradeBtn.GetComponent<UIDisabledButtonAlpha>();
            _disabledButtonAlpha.Init();

            _upgradeBtn.onClick.AddListener(Upgrade);
        }

        protected virtual void OnMoneyChanged(int count)
        {
            _disabledButtonAlpha.Interactable = count >= _currentPrice;
        }

        protected virtual void Upgrade()
        {
            _uiManager.ButtonClickSound();
        }

        protected virtual void OnEnable()
        {
            _gameManager.onMoneyChanged += OnMoneyChanged;

            var playerState = _gameManager.GameState.PlayerState;
            OnMoneyChanged(playerState.Money);
        }

        private void OnDisable()
        {
            _gameManager.onMoneyChanged -= OnMoneyChanged;
        }
    }
}