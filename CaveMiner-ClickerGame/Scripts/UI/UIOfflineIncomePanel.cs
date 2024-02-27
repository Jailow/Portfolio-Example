using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaveMiner.Helpers;
using System.Linq;

namespace CaveMiner.UI
{
    public class UIOfflineIncomePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelTr;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _takeButton;
        [SerializeField] private UIOfflineIncomeDropItem _dropItemPrefab;
        [SerializeField] private TextMeshProUGUI _experience;
        [SerializeField] private TextMeshProUGUI _minerals;
        [SerializeField] private Transform _resourcesParent;
        [SerializeField] private Transform _keysParent;
        [SerializeField] private TextMeshProUGUI _commonKeyCount;
        [SerializeField] private TextMeshProUGUI _rareKeyCount;
        [SerializeField] private TextMeshProUGUI _epicKeyCount;
        [SerializeField] private TextMeshProUGUI _mythicalKeyCount;
        [SerializeField] private TextMeshProUGUI _legendaryKeyCount;
        [SerializeField] private TextMeshProUGUI _infoText;

        private GameManager _gameManager;
        private UIManager _uiManager;

        public bool IsOpened { get; private set; }

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _uiManager = FindObjectOfType<UIManager>();

            _backgroundCloseButton.onClick.AddListener(Hide);

            _takeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Hide();
            });
        }

        public void Show(OfflineIncomeState offlineIncomeState)
        {
            gameObject.SetActive(true);

            _experience.gameObject.SetActive(offlineIncomeState.Experience > 0);
            _minerals.gameObject.SetActive(offlineIncomeState.Minerals > 0);
            _resourcesParent.gameObject.SetActive(offlineIncomeState.Resources.Count > 0);
            _keysParent.gameObject.SetActive(offlineIncomeState.KeyCount() > 0);

            _experience.text = $"{NumberToString.Convert(offlineIncomeState.Experience)} {I2.Loc.LocalizationManager.GetTranslation("OfflineIncome/experience")}";
            _minerals.text = $"{NumberToString.Convert(offlineIncomeState.Minerals)} {I2.Loc.LocalizationManager.GetTranslation("OfflineIncome/minerals")}";

            foreach(var resource in offlineIncomeState.Resources)
            {
                var resourceData = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == resource.Id);

                if (resourceData == null)
                    continue;

                var item = Instantiate(_dropItemPrefab, _resourcesParent, false);
                item.Set(resourceData.Icon, resource.Count);
            }

            _commonKeyCount.transform.parent.gameObject.SetActive(offlineIncomeState.CommonKey > 0);
            _commonKeyCount.text = offlineIncomeState.CommonKey.ToString();

            _rareKeyCount.transform.parent.gameObject.SetActive(offlineIncomeState.RareKey > 0);
            _rareKeyCount.text = offlineIncomeState.RareKey.ToString();

            _epicKeyCount.transform.parent.gameObject.SetActive(offlineIncomeState.EpicKey > 0);
            _epicKeyCount.text = offlineIncomeState.EpicKey.ToString();

            _mythicalKeyCount.transform.parent.gameObject.SetActive(offlineIncomeState.MythicalKey > 0);
            _mythicalKeyCount.text = offlineIncomeState.MythicalKey.ToString();

            _legendaryKeyCount.transform.parent.gameObject.SetActive(offlineIncomeState.LegendaryKey > 0);
            _legendaryKeyCount.text = offlineIncomeState.LegendaryKey.ToString();

            string timeLoc = I2.Loc.LocalizationManager.GetTranslation("OfflineIncome/time");
            string incomeLoc = I2.Loc.LocalizationManager.GetTranslation("OfflineIncome/income");
            string hourLoc = I2.Loc.LocalizationManager.GetTranslation("OfflineIncome/hour");

            var playerState = _gameManager.GameState.PlayerState;

            var maxOfflineTimeRebirth = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "max_offline_time");
            var maxOfflineTimeRebirthState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "max_offline_time");

            var offlineIncomeMultiplierRebirth = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "offline_income_multiplier");
            var offlineIncomeMultiplierRebirthState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "offline_income_multiplier");

            float offlineTime = ServerTimeManager.Instance.ServerTime - playerState.LastSaveTimestamp;
            float maxOfflineTime = (_gameManager.GameData.DefaultMaxOfflineTime) + ((maxOfflineTimeRebirth.Value * maxOfflineTimeRebirthState.Level) * 60f);
            float offlineIncomeMultiplier = _gameManager.GameData.DefaultOfflineIncomeMultiplier + ((offlineIncomeMultiplierRebirth.Value * offlineIncomeMultiplierRebirthState.Level) * 0.01f);

            string timeColor = offlineTime >= maxOfflineTime ? "F1002E" : "FFE272";

            offlineTime = Mathf.Clamp(offlineTime, 0, maxOfflineTime);
            _infoText.text = $"{timeLoc} <color=#{timeColor}>{(offlineTime / 60f / 60f).ToString("0.##")}/{(maxOfflineTime / 60f / 60f).ToString("0.##")}{hourLoc}</color> {incomeLoc} <color=#FFE272>{Mathf.RoundToInt(offlineIncomeMultiplier * 100f)}%</color>";

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelTr);
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelTr);
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