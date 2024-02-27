using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UICaveSelectionPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private Transform _unlockTab;
        [SerializeField] private UIExperienceTab _unlockExperienceTab;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Transform _caveParent;
        [SerializeField] private UICaveTab _caveTabPrefab;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private UICaveTab _selectedCaveTab;
        private UIDisabledButtonAlpha _disabledAlphaButton;
        private List<UICaveTab> _caveTabs;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _caveTabs = new List<UICaveTab>();

            _disabledAlphaButton = _unlockButton.GetComponent<UIDisabledButtonAlpha>();
            _disabledAlphaButton.Init();

            _unlockExperienceTab.Init(gameManager);

            _unlockButton.onClick.AddListener(UnlockCave);
            _closeButton.onClick.AddListener(() => 
            {
                _uiManager.ButtonClickSound();
                Hide();
            });
            _backgroundCloseButton.onClick.AddListener(Hide);

            UpdateCaveTabs();
        }

        private void UpdateCaveTabs()
        {
            foreach(var cave in _caveTabs)
            {
                cave.gameObject.SetActive(false);
            }

            int caveCount = 0;

            foreach (var cave in _gameManager.Caves)
            {
                caveCount++;

                if (caveCount > _gameManager.GameState.PlayerState.CaveLevel)
                    break;

                if (caveCount > _caveTabs.Count)
                {
                    var caveTab = Instantiate(_caveTabPrefab, _caveParent, false);
                    caveTab.Init(_gameManager, _uiManager, cave, OnSelect, OnOpenUpgrade);
                    _caveTabs.Add(caveTab);
                }

                _caveTabs[caveCount - 1].gameObject.SetActive(true);

                if (cave.Id == _gameManager.GameState.PlayerState.CurrentCave)
                {
                    _selectedCaveTab = _caveTabs[caveCount - 1];
                    _caveTabs[caveCount - 1].Select();
                }
                else
                {
                    _caveTabs[caveCount - 1].Deselect();
                }
            }
        }

        public void UnlockCave()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var needExperience = _gameManager.GetExperienceCountToNextLevel();

            if (playerState.Experience < needExperience)
                return;

            _uiManager.ButtonClickSound();

            _gameManager.UpgradeLevel();

            var cave = _gameManager.Caves[playerState.CaveLevel - 1];
            if (playerState.CaveLevel >= _caveTabs.Count)
            {
                var caveTab = Instantiate(_caveTabPrefab, _caveParent, false);
                caveTab.Init(_gameManager, _uiManager, cave, OnSelect, OnOpenUpgrade);
                _caveTabs.Add(caveTab);
            }

            OnSelect(_caveTabs[playerState.CaveLevel - 1]);

            UpdateUnlockTab();
        }

        public void OnSelect(UICaveTab caveTab)
        {
            if (_selectedCaveTab == caveTab)
                return;

            _selectedCaveTab?.Deselect();
            _selectedCaveTab = caveTab;
            caveTab.Select();

            _gameManager.SelectCave(caveTab.CaveData.Id);

            Hide();
        }

        public void OnOpenUpgrade(UICaveTab caveTab)
        {
            _uiManager.ShowCaveUpgradePanel(caveTab.CaveData);
        }

        private void UpdateUnlockTab()
        {
            var playerState = _gameManager.GameState.PlayerState;

            _unlockTab.gameObject.SetActive(playerState.CaveLevel < _gameManager.Caves.Length);

            if (playerState.CaveLevel > _gameManager.Caves.Length)
            {
                return;
            }

            int needCount = _gameManager.GetExperienceCountToNextLevel();

            _disabledAlphaButton.Interactable = playerState.Experience >= needCount;

            _unlockTab.SetSiblingIndex(int.MaxValue);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            UpdateCaveTabs();
            UpdateUnlockTab();
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