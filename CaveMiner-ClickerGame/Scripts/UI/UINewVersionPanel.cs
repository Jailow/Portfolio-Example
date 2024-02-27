using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UINewVersionPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _downloadBtn;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _backgroundBtn;

        private const string GOOGLE_PLAY_URL = "https://play.google.com/store/apps/details?id=com.TechJungle.CaveMinerClickerGame";

        public bool IsOpened { get; private set; }

        public void Init(UIManager uiManager)
        {
            _downloadBtn.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Application.OpenURL(GOOGLE_PLAY_URL);

                Hide();
            });

            _closeBtn.onClick.AddListener(Hide);
            _backgroundBtn.onClick.AddListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _description.text = I2.Loc.LocalizationManager.GetTranslation("Info/new_version_description").Replace("\\n", "\n");

            IsOpened = true;
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}