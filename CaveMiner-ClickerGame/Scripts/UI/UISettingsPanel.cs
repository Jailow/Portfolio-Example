using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UISettingsPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelTr;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _privacyPolicyButton;
        [SerializeField] private Button _achievementsButton;
        [SerializeField] private UILanguageSelectionTab _languageSelectionTab;
        [SerializeField] private UIGraphicsSelectionTab _graphicsSelectionTab;
        [SerializeField] private UIGooglePlayServicesButton _googlePlayServicesButton;
        [SerializeField] private UIToggle _vibrationToggle;
        [SerializeField] private UIToggle _soundToggle;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _languageSelectionTab.Init();
            _graphicsSelectionTab.Init(gameManager, _languageSelectionTab);

            _closeButton.onClick.AddListener(() => 
            {
                uiManager.ButtonClickSound();
                Hide();
            });

            _backgroundCloseButton.onClick.AddListener(Hide);

            _vibrationToggle.Init(gameManager.GameState.VibrationIsOn, () =>
            {
                uiManager.ButtonClickSound();
                gameManager.GameState.VibrationIsOn = !gameManager.GameState.VibrationIsOn;
            });

            _soundToggle.Init(gameManager.GameState.SoundsIsOn, () =>
            {
                uiManager.ButtonClickSound();
                gameManager.GameState.SoundsIsOn = !gameManager.GameState.SoundsIsOn;
            });

            _googlePlayServicesButton.Init(gameManager, uiManager);

            _privacyPolicyButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Application.OpenURL("https://tech-jungle-studio.tilda.ws/caveminer-clickergame/privacy-policy");
            });

            _achievementsButton.onClick.AddListener(() =>
            {
                Social.ShowAchievementsUI();
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private async void OnEnable()
        {
            IsOpened = true;

            _achievementsButton.gameObject.SetActive(Social.localUser.authenticated);

            await Task.Delay(5);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelTr);
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}