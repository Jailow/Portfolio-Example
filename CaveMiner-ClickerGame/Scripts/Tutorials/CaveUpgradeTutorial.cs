using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CaveMiner.UI;

namespace CaveMiner
{
    public class CaveUpgradeTutorial : TutorialBase
    {
        [SerializeField] private GameObject _lockControl;
        [SerializeField] private RectTransform _highlight;
        [SerializeField] private RectTransform _upButtonHighlightPos;
        [SerializeField] private RectTransform _caveButtonHighlightPos;
        [SerializeField] private UITutorialTab _caveButton;
        [SerializeField] private AnimationCurve _textTabsShowAnimationCurve;
        [SerializeField] private GameObject[] _textTabs;

        private bool _buttonClicked;

        private void OnButtonClick()
        {
            _buttonClicked = true;
        }

        protected override IEnumerator Tutorial()
        {
            _highlight.sizeDelta = new Vector2(Screen.width, Screen.height);

            yield return new WaitForSeconds(0.4f);

            var movePos = _upButtonHighlightPos.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 1f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_upButtonHighlightPos.sizeDelta, 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            _textTabs[0].transform.localScale = Vector3.zero;
            _textTabs[0].SetActive(true);
            _textTabs[0].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            movePos = _caveButtonHighlightPos.position;
            movePos.z = 0f;

            DOTween.Sequence().Append(_textTabs[0].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[0].SetActive(false));
            _highlight.DOMove(movePos, 1.2f).SetEase(Ease.OutSine);
            _highlight.DOSizeDelta(_caveButtonHighlightPos.sizeDelta, 1.2f).SetEase(Ease.OutSine);
            _caveButton.ShowAnimation();

            yield return new WaitForSeconds(1.2f);

            _textTabs[1].transform.localScale = Vector3.zero;
            _textTabs[1].SetActive(true);
            _textTabs[1].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            _lockControl.SetActive(false);
            var btn = _caveButton.GetComponent<Button>();
            btn.onClick.AddListener(OnButtonClick);

            while (!_buttonClicked)
                yield return null;

            btn.onClick.RemoveListener(OnButtonClick);

            DOTween.Sequence().Append(_textTabs[1].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[1].SetActive(false));
            _lockControl.SetActive(true);
            _highlight.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _highlight.anchoredPosition = Vector2.zero;

            _textTabs[2].transform.localScale = Vector3.zero;
            _textTabs[2].SetActive(true);
            _textTabs[2].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[2].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[2].SetActive(false));

            yield return new WaitForSeconds(0.35f);

            _textTabs[3].transform.localScale = Vector3.zero;
            _textTabs[3].SetActive(true);
            _textTabs[3].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[3].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[3].SetActive(false));
            _highlight.DOSizeDelta(new Vector2(Screen.width, Screen.height), 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}