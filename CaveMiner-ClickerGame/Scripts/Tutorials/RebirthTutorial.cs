using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class RebirthTutorial : TutorialBase
    {
        [SerializeField] private GameObject _lockControl;
        [SerializeField] private RectTransform _highlight;
        [SerializeField] private Image _rebirthButtonDisabledTexture;
        [SerializeField] private Button _rebirthButton;
        [SerializeField] private AnimationCurve _textTabsShowAnimationCurve;
        [SerializeField] private Transform _firstTextTabTr;
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

            var textTabPos = _rebirthButtonDisabledTexture.transform.position;
            textTabPos.z = 0f;

            _firstTextTabTr.position = textTabPos;

            var movePos = _rebirthButtonDisabledTexture.transform.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 1f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_rebirthButton.image.rectTransform.sizeDelta, 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            _rebirthButtonDisabledTexture.DOFade(0f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            _rebirthButtonDisabledTexture.gameObject.SetActive(false);
            _lockControl.SetActive(false);

            _textTabs[0].transform.localScale = Vector3.zero;
            _textTabs[0].SetActive(true);
            _textTabs[0].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            _rebirthButton.enabled = true;
            _rebirthButton.onClick.AddListener(OnButtonClicked);

            while (!_buttonClicked)
                yield return null;

            _rebirthButton.onClick.RemoveListener(OnButtonClicked);

            DOTween.Sequence().Append(_textTabs[0].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[0].SetActive(false));

            _highlight.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _highlight.anchoredPosition = Vector2.zero;

            _textTabs[1].transform.localScale = Vector3.zero;
            _textTabs[1].SetActive(true);
            _textTabs[1].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[1].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[1].SetActive(false));

            _highlight.DOMove(Vector2.zero, 1f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(new Vector2(Screen.width, Screen.height), 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}