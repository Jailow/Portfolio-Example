using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIToggle : MonoBehaviour
    {
        [SerializeField] private Sprite _handleOnSprite;
        [SerializeField] private Sprite _handleOffSprite;
        [SerializeField] private Sprite _backgroundOnSprite;
        [SerializeField] private Sprite _backgroundOffSprite;
        [SerializeField] private Image _handleImg;
        [SerializeField] private Image _backgroundImg;
        [SerializeField] private Vector2 _handleOffPosition;
        [SerializeField] private Vector2 _handleOnPosition;
        [SerializeField] private float _switchAnimationTime;

        private bool  _value;
        private Button _btn;
        private Action _onClick;
        private Tween _tween;

        public void Init(bool value, Action onClick)
        {
            _value = value;
            _onClick = onClick;

            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _value = !_value;

            _handleImg.sprite = _value ? _handleOnSprite : _handleOffSprite;
            _backgroundImg.sprite = _value ? _backgroundOnSprite : _backgroundOffSprite;

            _tween?.Kill();
            _tween = _handleImg.rectTransform.DOAnchorPos(_value ? _handleOnPosition : _handleOffPosition, _switchAnimationTime).SetEase(Ease.Linear).SetAutoKill(true);

            _onClick?.Invoke();
        }

        private void OnEnable()
        {
            _handleImg.sprite = _value ? _handleOnSprite : _handleOffSprite;
            _backgroundImg.sprite = _value ? _backgroundOnSprite : _backgroundOffSprite;
            _handleImg.rectTransform.anchoredPosition = _value ? _handleOnPosition : _handleOffPosition;
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _tween = null;
        }
    }
}