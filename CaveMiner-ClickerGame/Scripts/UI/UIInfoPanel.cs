using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

namespace CaveMiner.UI
{
    public class UIInfoPanel : MonoBehaviour
    {
        [SerializeField] private Button _acceptBtn;
        [SerializeField] private Button _declineBtn;
        [SerializeField] private TextMeshProUGUI _acceptBtnText;
        [SerializeField] private TextMeshProUGUI _declineBtnText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private Action _onAccept;
        private Action _onDecline;

        public bool IsOpened { get; private set; }

        public void Init(UIManager uiManager)
        {
            _acceptBtn.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                _onAccept?.Invoke();
                gameObject.SetActive(false);
            });

            _declineBtn.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                _onDecline?.Invoke();
                gameObject.SetActive(false);
            });
        }

        public void Show(string descriptionTextKey, string acceptBtnTextKey, string declineBtnTextKey, Action onAccept, Action onDecline, string[] args)
        {
            gameObject.SetActive(true);

            _onAccept = onAccept;
            _onDecline = onDecline;

            _acceptBtn.gameObject.SetActive(!string.IsNullOrEmpty(acceptBtnTextKey));
            _declineBtn.gameObject.SetActive(!string.IsNullOrEmpty(declineBtnTextKey));

            _descriptionText.text = string.Format(LocalizationManager.GetTranslation($"Info/{descriptionTextKey}"), args).Replace("\\n", "\n");

            if (!string.IsNullOrEmpty(acceptBtnTextKey))
                _acceptBtnText.text = LocalizationManager.GetTranslation($"Info/{acceptBtnTextKey}");

            if (!string.IsNullOrEmpty(declineBtnTextKey))
                _declineBtnText.text = LocalizationManager.GetTranslation($"Info/{declineBtnTextKey}");
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