using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GooglePlayGames.BasicApi;

namespace CaveMiner.UI
{
    public class UIGooglePlayServicesButton : MonoBehaviour
    {
        [SerializeField] private Sprite _activeButtonSprite;
        [SerializeField] private Sprite _deactiveButtonSprite;
        [SerializeField] private TextMeshProUGUI _buttonText;

        private Button _btn;
        private GameManager _gameManager;
        private UIManager _uiManager;

        private const string CONNECTED_KEY = "Settings/google_play_services_connected";
        private const string NOT_CONNECTED_KEY = "Settings/google_play_services_not_connected";

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(OnClick);

            //GooglePlayGamesManager.Instance.onAuthenticationCompleted += OnAuthenticationCompleted;
        }

        private void OnAuthenticationCompleted(SignInStatus status)
        {
            UpdateVisual();
        }

        private void OnClick()
        {
            //GooglePlayGamesManager.Instance.LogInGooglePlayServices();
        }

        private void UpdateVisual()
        {
            //if (GooglePlayGamesManager.Instance.ConnectedToGooglePlayServices)
            //{
            //    _btn.image.sprite = _activeButtonSprite;
            //    _buttonText.text = I2.Loc.LocalizationManager.GetTranslation(CONNECTED_KEY);
            //}
            //else
            //{
            //    _btn.image.sprite = _deactiveButtonSprite;
            //    _buttonText.text = I2.Loc.LocalizationManager.GetTranslation(NOT_CONNECTED_KEY);
            //}
        }

        private void OnEnable()
        {
            UpdateVisual();
        }
    }
}