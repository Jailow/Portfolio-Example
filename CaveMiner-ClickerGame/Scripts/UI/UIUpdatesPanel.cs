using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIUpdatesPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelTr;
        [SerializeField] private RectTransform _parentTr;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;

        public bool IsOpened { get; private set; }

        private GameManager _gameManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;

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

        private void Hide()
        {
            gameObject.SetActive(false);

            _gameManager.GameState.ViewedVersionUpdate = Application.version;
        }

        private async void OnEnable()
        {
            IsOpened = true;

            await Task.Delay(5);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelTr);

            var layoutGroups = _parentTr.GetComponentsInChildren<LayoutGroup>();
            foreach (var layoutGroup in layoutGroups)
                layoutGroup.enabled = false;

            var contentSizeFitters = _parentTr.GetComponentsInChildren<ContentSizeFitter>();
            foreach (var contentSizeFitter in contentSizeFitters)
                contentSizeFitter.enabled = false;

            var allTexts = _parentTr.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in allTexts)
                text.transform.SetParent(_parentTr);

            var allDots = _parentTr.GetComponentsInChildren<Image>();
            foreach (var dot in allDots)
                dot.transform.SetParent(_parentTr);
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}