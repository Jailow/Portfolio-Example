using UnityEngine;
using DG.Tweening;

namespace CaveMiner.UI
{
    public class UINextLevelAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _fadeRectTr;
        [SerializeField] private Vector2 _fadeStartPos;
        [SerializeField] private Vector2 _fadeEndPos;
        [SerializeField] private float _animationTime;
        [SerializeField] private float _waitTime;

        private Tween _animationTween;

        public void Show()
        {
            gameObject.SetActive(true);

            StartAnimation();
        }

        private void StartAnimation()
        {
            _fadeRectTr.anchoredPosition = _fadeStartPos;

            _animationTween = DOTween.Sequence()
                .Append(_fadeRectTr.DOAnchorPos(_fadeEndPos, _animationTime / 2f))
                .AppendInterval(_waitTime)
                .Append(_fadeRectTr.DOAnchorPos(_fadeStartPos, _animationTime / 2f)).
                AppendCallback(Hide);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            _animationTween?.Kill();
        }

        public void OnEnable()
        {
            StartAnimation();
        }
    }
}