using I2.Loc;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRebirthUpgradePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _effectText;
        [SerializeField] private TextMeshProUGUI _lockInfoText;
        [SerializeField] private TextMeshProUGUI _levelTabText;
        [SerializeField] private Image _levelTabFill;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;

        private LocalizationParamsManager _localizationParamsManager;
        private UIDisabledButtonAlpha _upgradeDisabledButton;
        private RebirthUpgrade _selectedRebirthUpgrade;
        private GameManager _gameManager;

        private const string HIGHTLIGHT_LOCK_INFO_COLOR = "FFE272";

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;

            _upgradeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                var playerState = _gameManager.GameState.PlayerState;
                if (playerState.RebirthPoints > 0)
                {
                    Dictionary<string, object> properties = new Dictionary<string, object>();
                    properties.Add("UpgradeId", _selectedRebirthUpgrade.Id);
                    AmplitudeManager.Instance.Event(AnalyticEventKey.REBIRTH_UPGRADE, properties);

                    _gameManager.AddRebirthUpgradeLevel(_selectedRebirthUpgrade.Id);
                    _gameManager.AddRebirthPoints(-1);

                    UpdateUpgradeInfo();
                }
            });

            _localizationParamsManager = GetComponent<LocalizationParamsManager>();
            _upgradeDisabledButton = _upgradeButton.GetComponent<UIDisabledButtonAlpha>();
            _upgradeDisabledButton.Init();

            _closeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Hide();
            });

            _backgroundButton.onClick.AddListener(Hide);
        }

        public void Show(RebirthUpgrade rebirthUpgrade)
        {
            _selectedRebirthUpgrade = rebirthUpgrade;

            gameObject.SetActive(true);

            var playerState = _gameManager.GameState.PlayerState;
            var rebirthUpgradeState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == rebirthUpgrade.Id);

            _icon.sprite = rebirthUpgrade.Icon;
            _nameText.text = LocalizationManager.GetTranslation($"Rebirth/{rebirthUpgrade.Id}_name");
            _descriptionText.text = LocalizationManager.GetTranslation($"Rebirth/{rebirthUpgrade.Id}_description");

            UpdateUpgradeInfo();

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void UpdateUpgradeInfo()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var rebirthUpgradeState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == _selectedRebirthUpgrade.Id);

            bool effectActive = _selectedRebirthUpgrade.MaxLevel > 1;
            bool levelTabActive = rebirthUpgradeState.Level < _selectedRebirthUpgrade.MaxLevel || _selectedRebirthUpgrade.MaxLevel <= 1;
            bool isUnlocked = CheckRebirthCondition(_selectedRebirthUpgrade);

            _levelText.text = _selectedRebirthUpgrade.MaxLevel > 1 ? string.Format(LocalizationManager.GetTranslation($"Rebirth/level"), rebirthUpgradeState.Level) : (rebirthUpgradeState.Level > 0 ? LocalizationManager.GetTranslation($"Rebirth/upgraded") : LocalizationManager.GetTranslation($"Rebirth/not_upgraded"));
            _upgradeButton.gameObject.SetActive(rebirthUpgradeState.Level < _selectedRebirthUpgrade.MaxLevel && isUnlocked);
            _upgradeDisabledButton.Interactable = playerState.RebirthPoints > 0;
            _lockInfoText.gameObject.SetActive(!isUnlocked);
            _effectText.transform.parent.gameObject.SetActive(effectActive);
            _levelTabFill.transform.parent.gameObject.SetActive(levelTabActive);

            if (!isUnlocked && _selectedRebirthUpgrade.Condition.ConditionType != RebirthConditionType.None)
            {
                var condition = _selectedRebirthUpgrade.Condition;
                switch (condition.ConditionType)
                {
                    case RebirthConditionType.RebirthUpgradeLevel:
                        string[] values = condition.Value.Split(';');
                        string rebirthUpgradeName = LocalizationManager.GetTranslation($"Rebirth/{values[0]}_name");
                        _lockInfoText.text = string.Format(LocalizationManager.GetTranslation($"Rebirth/Conditions/{condition.ConditionType}"), $"<color=#{HIGHTLIGHT_LOCK_INFO_COLOR}>{rebirthUpgradeName}</color>", $"<color=#{HIGHTLIGHT_LOCK_INFO_COLOR}>{values[1]}</color>");
                        break;
                    case RebirthConditionType.UnlockedCave:
                        string caveName = LocalizationManager.GetTranslation($"CaveSelection/Cave/{condition.Value}");
                        _lockInfoText.text = string.Format(LocalizationManager.GetTranslation($"Rebirth/Conditions/{condition.ConditionType}"), $"<color=#{HIGHTLIGHT_LOCK_INFO_COLOR}>{caveName}</color>");
                        break;
                    case RebirthConditionType.RebirthCount:
                        _localizationParamsManager.SetParameterValue("COUNT", condition.Value);
                        _lockInfoText.text = string.Format(LocalizationManager.GetTranslation($"Rebirth/Conditions/{condition.ConditionType}", true, 0, true, true, _localizationParamsManager.gameObject).Replace(condition.Value.ToString(), "0"), $"<color=#{HIGHTLIGHT_LOCK_INFO_COLOR}>{condition.Value}</color>");
                        break;
                    case RebirthConditionType.PickaxeDamageLevel:
                        _lockInfoText.text = string.Format(LocalizationManager.GetTranslation($"Rebirth/Conditions/{condition.ConditionType}"), $"<color=#{HIGHTLIGHT_LOCK_INFO_COLOR}>{condition.Value}</color>");
                        break;
                    default:
                        _lockInfoText.text = string.Format(LocalizationManager.GetTranslation($"Rebirth/Conditions/{condition.ConditionType}"), $"<color=#{HIGHTLIGHT_LOCK_INFO_COLOR}>{condition.Value}</color>");
                        break;
                }
            }

            if (effectActive)
            {
                double currentValue = rebirthUpgradeState.Level * _selectedRebirthUpgrade.Value;
                double nextValue = (rebirthUpgradeState.Level + 1) * _selectedRebirthUpgrade.Value;
                string effectFirstLine = $"{_selectedRebirthUpgrade.FirstSymbol}{currentValue.ToString("0.##")}{_selectedRebirthUpgrade.SecondSymbol} {LocalizationManager.GetTranslation($"Rebirth/{_selectedRebirthUpgrade.Id}_effect")}";
                string effectSecondLine = string.Empty;
                if(rebirthUpgradeState.Level < _selectedRebirthUpgrade.MaxLevel) effectSecondLine = $"{string.Format(LocalizationManager.GetTranslation($"Rebirth/next_level"), _selectedRebirthUpgrade.FirstSymbol, nextValue.ToString("0.##"), _selectedRebirthUpgrade.SecondSymbol)}";
                _effectText.text = $"{effectFirstLine}\n{effectSecondLine}";
            }

            if (levelTabActive)
            {
                _levelTabText.text = $"{rebirthUpgradeState.Level}/{_selectedRebirthUpgrade.MaxLevel}";
                _levelTabFill.fillAmount = (float)rebirthUpgradeState.Level / (float)_selectedRebirthUpgrade.MaxLevel;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
        }

        private bool CheckRebirthCondition(RebirthUpgrade rebirthUpgrade)
        {
            var condition = rebirthUpgrade.Condition;

            if (condition == null || condition.ConditionType == RebirthConditionType.None)
            {
                return true;
            }

            var playerState = _gameManager.GameState.PlayerState;

            int intValue = 0;
            string stringValue = string.Empty;


            switch (condition.ConditionType)
            {
                case RebirthConditionType.UnlockedCave:
                    if (!playerState.CaveStates.Any(e => e.Id == condition.Value && e.Level >= 1))
                        return false;
                    break;
                case RebirthConditionType.RebirthUpgradeLevel:
                    string[] rebirthUpgradeLevelValues = condition.Value.Split(';');
                    intValue = int.Parse(rebirthUpgradeLevelValues[1]);

                    var rebirthUpgradeState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == rebirthUpgradeLevelValues[0]);
                    if (rebirthUpgradeState != null)
                    {
                        if (rebirthUpgradeState.Level < intValue)
                            return false;
                    }
                    break;
                case RebirthConditionType.PickaxeDamageLevel:
                    intValue = int.Parse(condition.Value);
                    if (playerState.PickaxeState.MaxDamageLevel < intValue)
                        return false;
                    break;
                case RebirthConditionType.PickaxeWealthLevel:
                    intValue = int.Parse(condition.Value);
                    if (playerState.PickaxeState.MaxWealthLevel < intValue)
                        return false;
                    break;
                case RebirthConditionType.PickaxeExperienceLevel:
                    intValue = int.Parse(condition.Value);
                    if (playerState.PickaxeState.MaxExperienceLevel < intValue)
                        return false;
                    break;
                case RebirthConditionType.PickaxeAutoClickLevel:
                    intValue = int.Parse(condition.Value);
                    if (playerState.PickaxeState.MaxAutoClickLevel < intValue)
                        return false;
                    break;
                case RebirthConditionType.RebirthCount:
                    intValue = int.Parse(condition.Value);
                    if (playerState.Stats.RebirthCount < intValue)
                        return false;
                    break;
            }

            return true;
        }

        private void OnAddedRebirthUpgradeLevel(string upgradeId)
        {
            if (upgradeId != _selectedRebirthUpgrade.Id)
                return;

            UpdateUpgradeInfo();

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _gameManager.onAddedRebirthUpgradeLevel += OnAddedRebirthUpgradeLevel;

            IsOpened = true;
        }

        private void OnDisable()
        {
            _gameManager.onAddedRebirthUpgradeLevel -= OnAddedRebirthUpgradeLevel;

            IsOpened = false;
        }
    }
}