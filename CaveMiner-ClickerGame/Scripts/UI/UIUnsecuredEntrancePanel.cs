using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIUnsecuredEntrancePanel : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private TextMeshProUGUI _description;

        private const string DESCRIPTION_KEY = "unsecured_entrance";

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