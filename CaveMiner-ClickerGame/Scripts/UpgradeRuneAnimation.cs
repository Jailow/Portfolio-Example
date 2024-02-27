using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CaveMiner.UI;

namespace CaveMiner
{
    public class UpgradeRuneAnimation : MonoBehaviour
    {
        [SerializeField] private Image _currentRune;
        [SerializeField] private Image _additionalRune;
        [SerializeField] private Vector2 _currentRuneSizeDelta;
        [SerializeField] private Vector2 _additionalRuneSizeDelta;
        [SerializeField] private RectTransform _currentRuneTr;
        [SerializeField] private RectTransform _additionalRuneTr;
        [SerializeField] private float _animationTime;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float _hideAnimationTime;
        [SerializeField] private float _runeShakeTime;
        [SerializeField] private float _runeShakeStrength;
        [SerializeField] private int _runeShakeVibrato;
        [SerializeField] private AnimationCurve _runeShakeCurve;
        [SerializeField] private GameObject _slicedRuneObj;
        [SerializeField] private Image[] _slicedRuneParts;

        private GameManager _gameManager;
        private UIManager _uiManager;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;
        }

        public void Show(Sprite runeIcon, Sprite additionalIcon, int currentRuneLevel, ItemType itemType, bool isBroken, Action onComplete)
        {
            gameObject.SetActive(true);

            _currentRune.sprite = runeIcon;
            _additionalRune.sprite = additionalIcon;

            StartCoroutine(Animation(currentRuneLevel, itemType, isBroken, onComplete));
        }

        private void ResetAll()
        {
            foreach (var img in _slicedRuneParts)
            {
                img.color = Color.white;
            }

            _slicedRuneObj.SetActive(false);

            _currentRune.gameObject.SetActive(true);
            _additionalRune.gameObject.SetActive(true);
            _additionalRune.rectTransform.localScale = Helpers.CachedVector3.One;

            _additionalRune.rectTransform.position = _additionalRuneTr.position;

            _currentRune.rectTransform.sizeDelta = _currentRuneSizeDelta;
            _additionalRune.rectTransform.sizeDelta = _additionalRuneSizeDelta;
        }

        private IEnumerator Animation(int currentRuneLevel, ItemType itemType, bool isBroken, Action onComplete)
        {
            if (_gameManager.GameState.PlayerState.CaveStates.Count >= 2)
                CleverAdsSolutionsManager.Instance.EnableTimer = false;

            ResetAll();

            if (_gameManager.GameState.VibrationIsOn)
                Vibration.Vibrate(25);

            _additionalRune.rectTransform.DOScale(1.1f, _runeShakeTime / 3f).SetEase(_runeShakeCurve).SetAutoKill(true);
            _additionalRune.rectTransform.DOShakeAnchorPos(_runeShakeTime, _runeShakeStrength, (int)(_runeShakeVibrato / 2f), 90, false, true, ShakeRandomnessMode.Harmonic).SetAutoKill(true);

            yield return new WaitForSeconds(_runeShakeTime);

            _additionalRune.rectTransform.DOMove(_currentRune.rectTransform.position, _animationTime).SetEase(_animationCurve).SetAutoKill(true);
            _additionalRune.rectTransform.DOScale(1.35f, _animationTime).SetEase(_runeShakeCurve).SetAutoKill(true);

            yield return new WaitForSeconds(_animationTime);

            _additionalRune.gameObject.SetActive(false);

            if (isBroken)
            {
                if (_gameManager.GameState.VibrationIsOn)
                    Vibration.Vibrate(100);

                _slicedRuneObj.SetActive(true);

                switch (itemType)
                {
                    case ItemType.Rune:
                        if (currentRuneLevel <= 1)
                            _currentRune.gameObject.SetActive(false);
                        break;
                    case ItemType.Hammer:
                        if (isBroken)
                            _currentRune.gameObject.SetActive(false);
                        break;
                }

                yield return new WaitForSeconds(0.5f);

                foreach (var img in _slicedRuneParts)
                {
                    img.DOFade(0f, 0.6f).SetEase(Ease.Linear).SetAutoKill(true);
                }

                yield return new WaitForSeconds(0.6f);

                switch (itemType)
                {
                    case ItemType.Rune:
                        _uiManager.RuneUpgradePanel.RuneLevelRoller.PrevLevel();
                        break;
                    case ItemType.Hammer:
                        _uiManager.RuneUpgradePanel.RuneLevelRoller.ZeroLevel();
                        break;
                }

                if (currentRuneLevel <= 1)
                    yield return new WaitForSeconds(0.4f);
            }
            else
            {
                if (_gameManager.GameState.VibrationIsOn)
                    Vibration.Vibrate(35);

                _currentRune.rectTransform.DOScale(1.1f, _runeShakeTime / 3f).SetEase(_runeShakeCurve).SetAutoKill(true);
                _currentRune.rectTransform.DOShakeAnchorPos(_runeShakeTime, _runeShakeStrength, _runeShakeVibrato, 90, false, true, ShakeRandomnessMode.Harmonic).SetAutoKill(true);

                yield return new WaitForSeconds(_runeShakeTime);

                switch (itemType)
                {
                    case ItemType.Rune:
                        _uiManager.RuneUpgradePanel.RuneLevelRoller.NextLevel();
                        break;
                }
            }

            yield return new WaitForSeconds(0.45f);

            gameObject.SetActive(false);
            onComplete?.Invoke();

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