using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class CombineRuneAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform[] _runeSlots;
        [SerializeField] private RectTransform _inventoryButton;
        [SerializeField] private Image _firstRuneIcon;
        [SerializeField] private Image _secondRuneIcon;
        [SerializeField] private AnimationCurve _moveCurve;
        [SerializeField] private AnimationCurve _scaleCurve;
        [SerializeField] private float _animationTime;

        private GameManager _gameManager;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Show(Sprite icon, Action onCompleted)
        {
            gameObject.SetActive(true);

            StartCoroutine(Animation(icon, onCompleted));
        }

        private void ResetAll()
        {
            _firstRuneIcon.gameObject.SetActive(true);
            _secondRuneIcon.gameObject.SetActive(true);
            _firstRuneIcon.transform.localScale = Helpers.CachedVector3.One;
            _secondRuneIcon.transform.localScale = Helpers.CachedVector3.One;
        }

        private IEnumerator Animation(Sprite icon, Action onCompleted)
        {
            if (_gameManager.GameState.PlayerState.CaveStates.Count >= 2)
                CleverAdsSolutionsManager.Instance.EnableTimer = false;

            _firstRuneIcon.sprite = icon;
            _secondRuneIcon.sprite = icon;
            _firstRuneIcon.transform.position = _runeSlots[0].position;
            _secondRuneIcon.transform.position = _runeSlots[1].position;

            ResetAll();

            yield return new WaitForSeconds(0.1f);

            _firstRuneIcon.rectTransform.DOMoveX(0f, _animationTime * 0.6f).SetEase(_moveCurve).SetAutoKill(true);
            _secondRuneIcon.rectTransform.DOMoveX(0f, _animationTime * 0.6f).SetEase(_moveCurve).SetAutoKill(true);      

            yield return new WaitForSeconds(_animationTime * 0.6f);

            _secondRuneIcon.rectTransform.DOShakeAnchorPos(0.4f, 20, 8, 90, false, true, ShakeRandomnessMode.Harmonic).SetAutoKill(true);

            _firstRuneIcon.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.4f);

            _secondRuneIcon.transform.DOMove(_inventoryButton.position, _animationTime).SetEase(_moveCurve).SetAutoKill(true);
            _secondRuneIcon.transform.DOScale(0f, _animationTime).SetEase(_scaleCurve).SetAutoKill(true);

            yield return new WaitForSeconds(_animationTime);

            gameObject.SetActive(false);

            onCompleted?.Invoke();

            if (_gameManager.GameState.PlayerState.CaveStates.Count >= 2)
                CleverAdsSolutionsManager.Instance.EnableTimer = true;
        }

        private void OnEnable()
        {
            IsOpened = true;
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}