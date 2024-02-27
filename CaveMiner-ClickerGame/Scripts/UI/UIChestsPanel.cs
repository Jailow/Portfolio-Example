using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIChestsPanel : MonoBehaviour
    {
        [SerializeField] private UIChestTab[] _chestTabs;
        [SerializeField] private UIToggle _quickOpeningToggle;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;

        private GameManager _gameManager;
        private UIManager _uiManager;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _closeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Hide();
            });

            _backgroundCloseButton.onClick.AddListener(Hide);

            _quickOpeningToggle.Init(gameManager.GameState.QuickOpeningIsOn, () =>
            {
                uiManager.ButtonClickSound();
                gameManager.GameState.QuickOpeningIsOn = !gameManager.GameState.QuickOpeningIsOn;
            });

            foreach (var tab in _chestTabs)
            {
                tab.Init(gameManager, uiManager);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);

            bool keyPanelTutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("key_panel_tutorial");
            bool navigationPanelTutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("navigation_panel_tutorial");
            if (!TutorialController.Instance.TutorialPlaying && !navigationPanelTutorialCompleted && keyPanelTutorialCompleted)
            {
                if (!_uiManager.HaveOverlayPanel() && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                    TutorialController.Instance.StartTutorial("navigation_panel_tutorial");
            }
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