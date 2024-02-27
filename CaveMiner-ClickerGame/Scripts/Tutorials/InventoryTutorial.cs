using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class InventoryTutorial : TutorialBase
    {
        [SerializeField] private GameObject _lockControl;
        [SerializeField] private RectTransform _highlight;
        [SerializeField] private Image _inventoryButtonDisabledTexture;
        [SerializeField] private RectTransform _resourcesButtonTr;
        [SerializeField] private RectTransform _craftButtonTr;
        [SerializeField] private RectTransform _inventoryButtonTr;
        [SerializeField] private Button _inventoryButton;
        [SerializeField] private AnimationCurve _textTabsShowAnimationCurve;
        [SerializeField] private Transform _firstTextTabTr;
        [SerializeField] private GameObject[] _textTabs;
        [SerializeField] private Button _resourceCategoryButton;
        [SerializeField] private RectTransform _storageButton;

        private bool _buttonClicked;

        private void OnButtonClicked()
        {
            _buttonClicked = true;
        }

        protected override IEnumerator Tutorial()
        {
            _highlight.sizeDelta = new Vector2(Screen.width, Screen.height);

            yield return new WaitForSeconds(0.4f);

            _firstTextTabTr.position = _inventoryButtonDisabledTexture.transform.position;

            var movePos = _inventoryButtonDisabledTexture.transform.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 1f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_inventoryButton.image.rectTransform.sizeDelta, 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            _inventoryButtonDisabledTexture.DOFade(0f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            _inventoryButtonDisabledTexture.gameObject.SetActive(false);
            _lockControl.SetActive(false);

            _textTabs[0].transform.localScale = Vector3.zero;
            _textTabs[0].SetActive(true);
            _textTabs[0].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            _inventoryButton.enabled = true;
            _inventoryButton.onClick.AddListener(OnButtonClicked);

            while (!_buttonClicked)
                yield return null;

            _lockControl.SetActive(true);
            _inventoryButton.onClick.RemoveListener(OnButtonClicked);

            DOTween.Sequence().Append(_textTabs[0].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[0].SetActive(false));

            _highlight.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _highlight.anchoredPosition = Vector2.zero;

            _textTabs[1].transform.localScale = Vector3.zero;
            _textTabs[1].SetActive(true);
            _textTabs[1].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[1].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[1].SetActive(false));

            movePos = _inventoryButtonTr.position;
            movePos.z = 0f;

            _highlight.DOMove(movePos, 0.7f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_inventoryButtonTr.sizeDelta, 0.7f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.7f);

            _textTabs[2].transform.localScale = Vector3.zero;
            _textTabs[2].SetActive(true);
            _textTabs[2].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            movePos = _resourcesButtonTr.position;
            movePos.z = 0f;

            DOTween.Sequence().Append(_textTabs[2].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[2].SetActive(false));

            _highlight.DOMove(movePos, 0.5f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_resourcesButtonTr.sizeDelta, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _buttonClicked = false;
            _lockControl.SetActive(false);

            _textTabs[3].transform.localScale = Vector3.zero;
            _textTabs[3].SetActive(true);
            _textTabs[3].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            _resourceCategoryButton.onClick.AddListener(OnButtonClicked);

            while (!_buttonClicked)
                yield return null;

            _resourceCategoryButton.onClick.RemoveListener(OnButtonClicked);
            _lockControl.SetActive(true);

            yield return null;

            movePos = _storageButton.position;
            movePos.z = 0f;

            DOTween.Sequence().Append(_textTabs[3].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[3].SetActive(false));

            _highlight.DOMove(movePos, 0.5f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_storageButton.sizeDelta, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _textTabs[4].transform.localScale = Vector3.zero;
            _textTabs[4].SetActive(true);
            _textTabs[4].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            movePos = _craftButtonTr.position;
            movePos.z = 0f;

            DOTween.Sequence().Append(_textTabs[4].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[3].SetActive(false));
            _highlight.DOMove(movePos, 0.5f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(_craftButtonTr.sizeDelta, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _textTabs[5].transform.localScale = Vector3.zero;
            _textTabs[5].SetActive(true);
            _textTabs[5].transform.DOScale(1f, 0.3f).SetEase(_textTabsShowAnimationCurve);

            yield return WaitScreenTap();

            DOTween.Sequence().Append(_textTabs[5].transform.DOScale(0f, 0.3f).SetEase(Ease.Flash)).AppendCallback(() => _textTabs[5].SetActive(false));

            _highlight.DOSizeDelta(Vector2.zero, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);

            _highlight.anchoredPosition = Vector2.zero;
            _highlight.DOMove(Vector2.zero, 1f).SetEase(Ease.InOutSine);
            _highlight.DOSizeDelta(new Vector2(Screen.width, Screen.height), 1f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}