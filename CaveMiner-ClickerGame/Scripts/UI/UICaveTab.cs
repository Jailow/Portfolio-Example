using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace CaveMiner.UI
{
    public class UICaveTab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _averagePriceText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _checkMark;
        [SerializeField] private Button _arrowBtn;

        private Button _button;
        private GameManager _gameManager;

        public CaveData CaveData { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager, CaveData caveData, Action<UICaveTab> onSelect, Action<UICaveTab> onOpenUpgrade)
        {
            _gameManager = gameManager;

            _button = GetComponent<Button>();

            _button.onClick.AddListener(() => 
            {
                uiManager.ButtonClickSound();
                onSelect?.Invoke(this);
            });

            _arrowBtn.onClick.AddListener(() =>
            {
                onOpenUpgrade?.Invoke(this);
            });

            CaveData = caveData;

            _icon.sprite = caveData.Icon;
            _nameText.text = I2.Loc.LocalizationManager.GetTranslation($"CaveSelection/Cave/{CaveData.Id}");

            if(gameObject.activeSelf)
                _gameManager.onCaveLevelChanged += OnCaveLevelChanged;

            UpdateLevel();
        }

        public void Select()
        {
            _checkMark.SetActive(true);
        }

        public void Deselect()
        {
            _checkMark.SetActive(false);
        }

        private void OnCaveLevelChanged(string caveId)
        {
            if (CaveData == null || CaveData.Id != caveId)
                return;

            UpdateLevel();
        }

        private void UpdateLevel()
        {
            var caveState = _gameManager.GetCaveState(CaveData.Id);

            _averagePriceText.text = CaveData.GetAveragePricePerBlock(caveState.Level).ToString("0.##");
            _levelText.text = string.Format(I2.Loc.LocalizationManager.GetTranslation($"CaveSelection/level"), caveState.Level);
        }

        private void OnEnable()
        {
            if(_gameManager != null)
                _gameManager.onCaveLevelChanged += OnCaveLevelChanged;

            if (CaveData == null)
                return;

            _nameText.text = I2.Loc.LocalizationManager.GetTranslation($"CaveSelection/Cave/{CaveData.Id}");

            UpdateLevel();
        }

        private void OnDisable()
        {
            _gameManager.onCaveLevelChanged -= OnCaveLevelChanged;
        }
    }
}