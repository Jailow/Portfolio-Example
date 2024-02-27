using DG.Tweening;
using System.Collections;
using UnityEngine;
using CaveMiner.UI;

namespace CaveMiner
{
    public class RunePanelTutorial : TutorialBase
    {
        [SerializeField] private GameObject _lockControl;
        [SerializeField] private RectTransform _highlight;
        [SerializeField] private RectTransform _runePanelPos;
        [SerializeField] private AnimationCurve _textTabsShowAnimationCurve;
        [SerializeField] private UITutorialTab _runePanel;
        [SerializeField] private GameObject[] _textTabs;

        protected override IEnumerator Tutorial()
        {
            _highlight.sizeDelta = new Vector2(Screen.width, Screen.height);

            yield return new WaitForSeconds(0.4f);

            _runePanel.ShowAnimation();

            yield return new WaitForSeconds(1f);

            var movePos = _runePanelPos.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 1f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_runePanelPos.sizeDelta, 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            _textTabs[0].transform.localScale = Vector3.zero;
            _textTabs[0].SetActive(true);
            _textTabs[0].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[0].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[0].SetActive(false));

            yield return new WaitForSeconds(0.3f);

            _textTabs[1].transform.localScale = Vector3.zero;
            _textTabs[1].SetActive(true);
            _textTabs[1].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[1].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[1].SetActive(false));

            yield return new WaitForSeconds(0.3f);

            _textTabs[2].transform.localScale = Vector3.zero;
            _textTabs[2].SetActive(true);
            _textTabs[2].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[2].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[2].SetActive(false));

            yield return new WaitForSeconds(0.3f);

            _textTabs[3].transform.localScale = Vector3.zero;
            _textTabs[3].SetActive(true);
            _textTabs[3].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[3].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[3].SetActive(false));

            _highlight.DOSizeDelta(Vector2.zero, 0.6f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.6f);

            _highlight.anchoredPosition = Vector2.zero;

            _highlight.DOSizeDelta(new Vector2(Screen.width, Screen.height), 1f).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}