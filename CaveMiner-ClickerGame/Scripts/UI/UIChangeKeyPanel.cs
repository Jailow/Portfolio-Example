using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIChangeKeyPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;

        private UIChangeKeyTab[] _changeKeyTabs;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _changeKeyTabs = GetComponentsInChildren<UIChangeKeyTab>(true);
            foreach (var tab in _changeKeyTabs)
                tab.Init(gameManager, uiManager);

            _closeButton.onClick.AddListener(() =>
                {
                    uiManager.ButtonClickSound();
                    Hide();
                });

            _backgroundCloseButton.onClick.AddListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
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