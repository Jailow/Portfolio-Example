using UnityEngine;
using DG.Tweening;

namespace CaveMiner.UI
{
    public class UIDiamondTextureBackground : MonoBehaviour
    {
        [SerializeField] private RectTransform _textureTr;
        [SerializeField] private float _animationTime;
        [SerializeField] private Vector2 _startPosition;
        [SerializeField] private Vector2 _endPosition;

        private Tween _animationTween;

        private void OnEnable()
        {
            _animationTween?.Kill();

            _textureTr.anchoredPosition = _startPosition;
            _animationTween = DOTween.Sequence().Append(_textureTr.DOLocalMove(_endPosition, _animationTime).SetEase(Ease.Linear)).SetLoops(-1);
        }

        private void OnDisable()
        {
            _animationTween?.Kill();
            _animationTween = null;
        }
    }
}