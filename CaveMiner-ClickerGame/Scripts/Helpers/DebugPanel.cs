using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.Helpers
{
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _openAndClosePanelBtn;
        [SerializeField] private Button _saveGameBtn;
        [Header("PickaxeUpgrades")]
        [SerializeField] private TextMeshProUGUI _speedUpgradeLevelText;
        [SerializeField] private TextMeshProUGUI _wealthUpgradeLevelText;
        [SerializeField] private TextMeshProUGUI _experienceUpgradeLevelText;
        [SerializeField] private TextMeshProUGUI _autoClickUpgradeLevelText;
        [SerializeField] private TextMeshProUGUI _rebirthPointsCountText;
        [SerializeField] private Button _speedUpgradeNextLevelBtn;
        [SerializeField] private Button _speedUpgradePrevLevelBtn;
        [SerializeField] private Button _wealthUpgradeNextLevelBtn;
        [SerializeField] private Button _wealthUpgradePrevLevelBtn;
        [SerializeField] private Button _experienceUpgradeNextLevelBtn;
        [SerializeField] private Button _experienceUpgradePrevLevelBtn;
        [SerializeField] private Button _autoClickUpgradeNextLevelBtn;
        [SerializeField] private Button _autoClickUpgradePrevLevelBtn;
        [SerializeField] private Button _rebirthPointsPlusBtn;
        [SerializeField] private Button _rebirthPointsMinusBtn;

        private bool _isOpened;
        private GameManager _gameManager;

        private void Awake()
        {
#if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
            gameObject.SetActive(false);
            return;
#endif

            _gameManager = FindObjectOfType<GameManager>();

            _panel.SetActive(false);
            _openAndClosePanelBtn.onClick.AddListener(OpenPanel);

            _saveGameBtn.onClick.AddListener(_gameManager.SaveGame);

            var playerState = _gameManager.GameState.PlayerState;

            _rebirthPointsCountText.text = playerState.RebirthPoints.ToString();

            _rebirthPointsPlusBtn.onClick.AddListener(() =>
            {
                _gameManager.AddRebirthPoints(1);
                _rebirthPointsCountText.text = playerState.RebirthPoints.ToString();
            });

            _rebirthPointsMinusBtn.onClick.AddListener(() =>
            {
                _gameManager.AddRebirthPoints(-1);
                _rebirthPointsCountText.text = playerState.RebirthPoints.ToString();
            });

            #region PickaxeUpgradeButtons
            _speedUpgradeNextLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddDamage(1);
                UpdatePickaxe();
            });

            _speedUpgradePrevLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddDamage(-1);
                UpdatePickaxe();
            });

            _wealthUpgradeNextLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddWealth(1);
                UpdatePickaxe();
            });

            _wealthUpgradePrevLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddWealth(-1);
                UpdatePickaxe();
            });

            _experienceUpgradeNextLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddExperience(1);
                UpdatePickaxe();
            });

            _experienceUpgradePrevLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddExperience(-1);
                UpdatePickaxe();
            });

            _autoClickUpgradeNextLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddAutoClick(1);
                UpdatePickaxe();
            });

            _autoClickUpgradePrevLevelBtn.onClick.AddListener(() =>
            {
                _gameManager.Pickaxe.AddAutoClick(-1);
                UpdatePickaxe();
            });
            #endregion
        }

        private void UpdatePickaxe()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var pickaxeState = playerState.PickaxeState;
            _speedUpgradeLevelText.text = pickaxeState.DamageLevel.ToString();
            _wealthUpgradeLevelText.text = pickaxeState.WealthLevel.ToString();
            _experienceUpgradeLevelText.text = pickaxeState.ExperienceLevel.ToString();
            _autoClickUpgradeLevelText.text = pickaxeState.AutoClickLevel.ToString();
        }

        private void OpenPanel()
        {
            _isOpened = !_isOpened;
            _panel.SetActive(_isOpened);

            if (_isOpened)
            {
                UpdatePickaxe();
            }
        }
    }
}