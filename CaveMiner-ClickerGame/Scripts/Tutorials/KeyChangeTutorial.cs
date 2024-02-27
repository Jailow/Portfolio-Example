using CaveMiner.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class KeyChangeTutorial : TutorialBase
    {
        [SerializeField] private GameObject _lockControl;
        [SerializeField] private RectTransform _changeKeyButtonHighlightPos;
        [SerializeField] private RectTransform _highlight;
        [SerializeField] private UITutorialTab _changeKeyButton;
        [SerializeField] private AnimationCurve _textTabsShowAnimationCurve;
        [SerializeField] private GameObject[] _textTabs;

        private bool _buttonClicked;

        private void OnButtonClicked()
        {
            _buttonClicked = true;
        }

        protected override IEnumerator Tutorial()
        {
            _highlight.sizeDelta = new Vector2(Screen.width, Screen.height);

            yield return new WaitForSeconds(0.4f);

            var movePos = _changeKeyButtonHighlightPos.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 1.2f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_changeKeyButtonHighlightPos.sizeDelta, 1.2f).SetEase(Ease.InOutSine);
            _changeKeyButton.ShowAnimation();
 
            yield return new WaitForSeconds(1.2f);

            _textTabs[0].transform.localScale = Vector3.zero;
            _textTabs[0].SetActive(true);
            _textTabs[0].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            _lockControl.SetActive(false);
            var btn = _changeKeyButton.GetComponent<Button>();
            btn.onClick.AddListener(OnButtonClicked);

            while (!_buttonClicked)
                yield return null;

            btn.onClick.RemoveListener(OnButtonClicked);

            _lockControl.SetActive(true);
            DOTween.Sequence().Append(_textTabs[0].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[0].SetActive(false));
            _highlight.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.OutSine);

            yield return new WaitForSeconds(0.5f);

            _textTabs[1].transform.localScale = Vector3.zero;
            _textTabs[1].SetActive(true);
            _textTabs[1].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[1].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[1].SetActive(false));
            _highlight.position = Vector2.zero;

            yield return new WaitForSeconds(0.4f);

            _highlight.DOSizeDelta(new Vector2(Screen.width, Screen.height), 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}