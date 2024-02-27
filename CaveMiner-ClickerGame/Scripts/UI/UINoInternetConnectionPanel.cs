using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

namespace CaveMiner.UI
{
    public class UINoInternetConnectionPanel : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private TextMeshProUGUI _description;

        private const string DESCRIPTION_KEY = "no_internet_connection";

        private void Awake()
        {
            _exitButton.onClick.AddListener(Application.Quit);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _description.text = LocalizationManager.GetTranslation($"Info/{DESCRIPTION_KEY}").Replace("\\n", "\n");
        }
    }
}