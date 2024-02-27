using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public enum NavigationButtonType
    {
        Shop,
        Inventory,
        Cave,
        Upgrades,
        Rebirth,
    }

    public class UINavigationButton : MonoBehaviour
    {
        [SerializeField] private NavigationButtonType _buttonType;
        [SerializeField] private GameObject _disabledTextureObj;
        [SerializeField] private Image _backgroundImg;
        [SerializeField] private Image _backgroundTextureImg;
        [SerializeField] private RectTransform _iconTr;
        [SerializeField] private float _iconNormalScale;
        [SerializeField] private float _iconActiveScale;
        [SerializeField] private Vector2 _iconNormalPosition;
        [SerializeField] private Vector2 _iconActivePosition;
        [SerializeField] private AnimationCurve _curve;

        private Button _btn;
        private GameManager _gameManager;
        private Color _backgroundColor;
        private Color _backgroundTextureColor;
        private const float _transitionTime = 0.2f;

        public NavigationButtonType ButtonType => _buttonType;

        public void Init(GameManager gameManager, Action<UINavigationButton, bool> onClick)
        {
            _gameManager = gameManager;

            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(() => 
            {
                onClick?.Invoke(this, false);
            });

            _backgroundColor = _backgroundImg.color;
            _backgroundTextureColor = _backgroundTextureImg.color;

            TutorialController.Instance.onTutorialCompleted += tutorialId => UpdateButton();

            UpdateButton();
        }

        private void UpdateButton()
        {
            var playerState = _gameManager.GameState.PlayerState;

            switch (_buttonType)
            {
                case NavigationButtonType.Shop:
                    bool shopTutorialCompleted = playerState.CompletedTutorialsIds.Contains("shop_tutorial");
                    _disabledTextureObj.SetActive(!shopTutorialCompleted);
                    break;
                case NavigationButtonType.Inventory:
                    bool inventoryTutorial = playerState.CompletedTutorialsIds.Contains("inventory_tutorial");
                    _disabledTextureObj.SetActive(!inventoryTutorial);
                    break;
                case NavigationButtonType.Upgrades:
                    bool upgradesTutorial = playerState.CompletedTutorialsIds.Contains("upgrades_tutorial");
                    _disabledTextureObj.SetActive(!upgradesTutorial);
                    break;
                case NavigationButtonType.Rebirth:
                    bool rebirthTutorial = playerState.CompletedTutorialsIds.Contains("rebirth_tutorial");
                    _disabledTextureObj.SetActive(!rebirthTutorial);
                    break;
            }

            if (_buttonType != NavigationButtonType.Cave)
                _btn.enabled = !_disabledTextureObj.activeSelf;
        }

        public void Select()
        {
            _backgroundColor.a = 1f;
            _backgroundTextureColor.a = 0f;

            _backgroundImg.DOColor(_backgroundColor, _transitionTime).SetAutoKill(true);
            _backgroundTextureImg.DOColor(_backgroundTextureColor, _transitionTime).SetAutoKill(true);
            _iconTr.DOScale(_iconActiveScale, _transitionTime).SetEase(_curve).SetAutoKill(true);
            _iconTr.DOLocalMove(_iconActivePosition, _transitionTime).SetEase(_curve).SetAutoKill(true);
        }

        public void SelectInstante()
        {
            _backgroundColor.a = 1f;
            _backgroundTextureColor.a = 0f;

            _backgroundImg.color = _backgroundColor;
            _backgroundTextureImg.color = _backgroundTextureColor;
            _iconTr.localScale = Helpers.CachedVector3.One * _iconActiveScale;
            _iconTr.anchoredPosition = _iconActivePosition;
        }

        public void Deselect()
        {
            _backgroundColor.a = 0f;
            _backgroundTextureColor.a = 1f;

            _backgroundImg.DOColor(_backgroundColor, _transitionTime).SetAutoKill(true);
            _backgroundTextureImg.DOColor(_backgroundTextureColor, _transitionTime).SetAutoKill(true);
            _iconTr.DOScale(_iconNormalScale, _transitionTime).SetEase(_curve).SetAutoKill(true);
            _iconTr.DOLocalMove(_iconNormalPosition, _transitionTime).SetEase(_curve).SetAutoKill(true);
        }
    }
}