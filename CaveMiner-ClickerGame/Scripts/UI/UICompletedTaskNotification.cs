using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public class UICompletedTaskNotification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private RectTransform _line;
        [SerializeField] private AnimationCurve _showCurve;

        private RectTransform _tr;
        private Action _onAnimationCompleted;

        public bool IsPlaying { get; private set; }

        public void Init(Action onAnimationCompleted)
        {
            _tr = GetComponent<RectTransform>();

            _onAnimationCompleted = onAnimationCompleted;
        }

        public void Show(string taskName)
        {
            IsPlaying = true;

            gameObject.SetActive(true);

            _title.text = taskName;
            _tr.localScale = CachedVector3.Up;
            _line.localScale = CachedVector3.Up;

            StopAllCoroutines();
            StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            _tr.DOScaleX(1f, 0.4f).SetEase(_showCurve);

            yield return new WaitForSeconds(1f);

            _line.DOScaleX(1f, 0.25f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.75f);

            _tr.DOScaleX(0f, 0.3f).SetEase(Ease.InSine);

            yield return new WaitForSeconds(0.3f);

            gameObject.SetActive(false);
            IsPlaying = false;

            _onAnimationCompleted?.Invoke();
        }
    }
}