using System.Collections;
using UnityEngine;
using CaveMiner.UI;
using UnityEngine.UI;
using DG.Tweening;

namespace CaveMiner
{
    public class KeyPanelTutorial : TutorialBase
    {
        [SerializeField] private GameObject _lockControl;
        [SerializeField] private RectTransform _keyPanelHighlightPos;
        [SerializeField] private RectTransform _chestButtonHighlightPos;
        [SerializeField] private RectTransform _highlight;
        [SerializeField] private UITutorialTab _keyPanelTab;
        [SerializeField] private UITutorialTab _chestButton;
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

            var movePos = _keyPanelHighlightPos.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 1.5f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_keyPanelHighlightPos.sizeDelta, 1.5f).SetEase(Ease.InOutSine);
            _keyPanelTab.ShowAnimation();

            yield return new WaitForSeconds(1.5f);

            _textTabs[0].transform.localScale = Vector3.zero;
            _textTabs[0].SetActive(true);
            _textTabs[0].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            movePos = _chestButtonHighlightPos.position;
            movePos.z = 0f;

            DOTween.Sequence().Append(_textTabs[0].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[0].SetActive(false));

            _highlight.DOMove(movePos, 1f).SetEase(Ease.OutSine);
            _highlight.DOSizeDelta(_chestButtonHighlightPos.sizeDelta, 1f).SetEase(Ease.OutSine);
            _chestButton.ShowAnimation();

            yield return new WaitForSeconds(1f);

            _textTabs[1].transform.localScale = Vector3.zero;
            _textTabs[1].SetActive(true);
            _textTabs[1].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            _lockControl.SetActive(false);

            var chestButton = _chestButton.GetComponent<Button>();
            chestButton.onClick.AddListener(OnButtonClicked);

            while (!_buttonClicked)
                yield return null;

            _highlight.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.OutSine);

            _lockControl.SetActive(true);
            DOTween.Sequence().Append(_textTabs[1].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[1].SetActive(false));
            chestButton.onClick.RemoveListener(OnButtonClicked);

            yield return new WaitForSeconds(0.5f);

            _highlight.anchoredPosition = Vector2.zero;
            _highlight.DOSizeDelta(new Vector2(Screen.width, Screen.height), 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}