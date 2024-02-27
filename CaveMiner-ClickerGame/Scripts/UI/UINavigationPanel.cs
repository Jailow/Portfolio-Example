using System.Linq;
using UnityEngine;

namespace CaveMiner.UI
{
    public class UINavigationPanel : MonoBehaviour
    {
        [SerializeField] private UIBlockHealthBar _healthBar;
        [SerializeField] private UICaveDepthBar _depthBar;
        [SerializeField] private RectTransform _inventoryIconTr;

        private UINavigationButton[] _navigationButtons;
        private UINavigationButton _currentSelectedButton;
        private UIManager _uiManager;
        private GameManager _gameManager;

        public UICaveDepthBar DepthBar => _depthBar;
        public UIBlockHealthBar HealthBar => _healthBar;
        public RectTransform InventoryIconTr => _inventoryIconTr;
        public NavigationButtonType CurrentSelectedNavigationButton { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _depthBar.Init(gameManager);

            _navigationButtons = GetComponentsInChildren<UINavigationButton>();

            for (int i = 0; i < _navigationButtons.Length; i++)
            {
                _navigationButtons[i].Init(gameManager, OnSelectButton);
            }

            OnSelectButton(_navigationButtons[Mathf.RoundToInt(_navigationButtons.Length / 2)], true);
        }

        public void SelectButton(NavigationButtonType type)
        {
            var navigationButton = _navigationButtons.FirstOrDefault(e => e.ButtonType == type);

            if (navigationButton == null)
                return;

            OnSelectButton(navigationButton, true);
        }

        private void OnSelectButton(UINavigationButton navigationButton, bool instante)
        {
            if (_currentSelectedButton == navigationButton)
                return;

            if (navigationButton.ButtonType == NavigationButtonType.Upgrades)
            {
                var playerState = _gameManager.GameState.PlayerState;
                if (!TutorialController.Instance.TutorialPlaying && !playerState.CompletedTutorialsIds.Contains("rune_panel_tutorial"))
                {
                    var haveRune = playerState.Items.Any(e => _gameManager.Items.FirstOrDefault(b => b.Id == e.Id).ItemType == ItemType.Rune);
                    if (playerState.CaveLevel >= 6 || haveRune)
                    {
                        if (!_uiManager.HaveOverlayPanel())
                            TutorialController.Instance.StartTutorial("rune_panel_tutorial");
                    }
                }
            }

            _currentSelectedButton?.Deselect();
            _currentSelectedButton = navigationButton;

            if (instante)
            {
                navigationButton.SelectInstante();
            }
            else
            {
                navigationButton.Select();
                _uiManager.ButtonClickSound();
            }

            CurrentSelectedNavigationButton = navigationButton.ButtonType;

            switch (navigationButton.ButtonType)
            {
                case NavigationButtonType.Shop:
                    _uiManager.ShowPanel(_uiManager.ShopScreen.gameObject);
                    break;
                case NavigationButtonType.Inventory:
                    _uiManager.ShowPanel(_uiManager.InventoryScreen.gameObject);
                    break;
                case NavigationButtonType.Cave:
                    _uiManager.ShowPanel(_uiManager.CaveScreen.gameObject);
                    break;
                case NavigationButtonType.Upgrades:
                    _uiManager.ShowPanel(_uiManager.UpgradesScreen.gameObject);
                    break;
                case NavigationButtonType.Rebirth:
                    _uiManager.ShowPanel(_uiManager.RebirthScreen.gameObject);
                    break;
            }
        }
    }
}