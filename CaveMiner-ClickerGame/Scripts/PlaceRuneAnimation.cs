using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace CaveMiner
{
    public class PlaceRuneAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform[] _runeSlots;
        [SerializeField] private Image _runeIcon;
        [SerializeField] private GameObject _slicedRune;
        [SerializeField] private Image[] _slicedRuneParts;
        [Header("Backgrounds")]
        [SerializeField] private Image[] _backgrounds;
        [SerializeField] private float _showScreenTime;
        [SerializeField] private AnimationCurve _showScreenCurve;
        [Header("Rune")]
        [SerializeField] private Image _whiteLight;
        [SerializeField] private float _whiteLightTime;
        [SerializeField] private float _whiteAlpha;
        [SerializeField] private AnimationCurve _whiteLightCurve;
        [SerializeField] private RectTransform _runeLightTr;
        [SerializeField] private float _showRuneLightTime;
        [SerializeField] private RectTransform _runeTr;
        [SerializeField] private float _showRuneTime;
        [SerializeField] private AnimationCurve _showRuneCurve;
        [SerializeField] private int _runeShakeCount;
        [SerializeField] private float _runeShakeTime;
        [SerializeField] private float _runeShakeStrength;
        [SerializeField] private int _runeShakeVibrato;
        [SerializeField] private float _runeFinalShakeTime;
        [SerializeField] private Vector2 _mainRuneSizeDelta;
        [SerializeField] private Vector2 _finalRuneSizeDelta;
        [SerializeField] private AnimationCurve _setRuneCurve;
        [SerializeField] private float _setRuneTime;

        private Color[] _backgroundColors;
        private GameManager _gameManager;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _backgroundColors = new Color[_backgrounds.Length];
            for(int i = 0; i < _backgroundColors.Length; i++)
            {
                _backgroundColors[i] = _backgrounds[i].color;
            }
        }

        public void Show(int slotIndex, bool isBroken, Sprite icon, Action onCompleted)
        {
            gameObject.SetActive(true);

            StartCoroutine(Animation(slotIndex, isBroken, icon, onCompleted));
        }

        private void ResetAll()
        {
            var clearWhite = new Color(1f, 1f, 1f, 0f);

            foreach (var img in _backgrounds)
            {
                img.color = Color.clear;
            }

            foreach(var img in _slicedRuneParts)
            {
                img.color = Color.white;
            }

            _runeIcon.color = Color.white;
            _runeTr.sizeDelta = _mainRuneSizeDelta;
            _runeTr.anchoredPosition = Helpers.CachedVector2.Zero;

            _slicedRune.SetActive(false);
            _runeIcon.gameObject.SetActive(true);

            _whiteLight.color = clearWhite;

            _runeTr.localScale = Helpers.CachedVector3.Zero;
            _runeLightTr.localScale = Helpers.CachedVector3.Zero;
        }

        private IEnumerator Animation(int slotIndex, bool isBroken, Sprite icon, Action onCompleted)
        {
            if (_gameManager.GameState.PlayerState.CaveStates.Count >= 2)
                CleverAdsSolutionsManager.Instance.EnableTimer = false;

            _runeIcon.sprite = icon;

            ResetAll();

            for(int i = 0; i < _backgrounds.Length; i++)
            {
                _backgrounds[i].DOColor(_backgroundColors[i], _showScreenTime).SetEase(_showScreenCurve).SetAutoKill();
            }

            yield return new WaitForSeconds(_showScreenTime);

            _runeLightTr.DOScale(1f, _showRuneLightTime).SetEase(Ease.Linear).SetAutoKill(true);

            yield return new WaitForSeconds(_showRuneLightTime / 2f);

            _runeTr.DOScale(1f, _showRuneTime).SetEase(_showRuneCurve).SetAutoKill(true);
            _runeTr.DOShakeAnchorPos(_showRuneTime * 1.5f, _runeShakeStrength, _runeShakeVibrato, 90, false, true, ShakeRandomnessMode.Harmonic).SetAutoKill(true);

            if (_gameManager.GameState.VibrationIsOn)
                Vibration.Vibrate(25);

            yield return new WaitForSeconds(_showRuneTime * 2f);

            for (int i = 0; i < _runeShakeCount; i++)
            {
                _runeTr.DOScale(1.1f, _runeShakeTime / 3f).SetEase(_whiteLightCurve).SetAutoKill(true);
                _runeTr.DOShakeAnchorPos(_runeShakeTime, _runeShakeStrength, _runeShakeVibrato, 90, false, true, ShakeRandomnessMode.Harmonic).SetAutoKill(true);
                _whiteLight.DOFade(_whiteAlpha, _whiteLightTime).SetEase(_whiteLightCurve).SetAutoKill(true);

                if (_gameManager.GameState.VibrationIsOn)
                    Vibration.Vibrate(25);

                yield return new WaitForSeconds(_runeShakeTime);
            }

            _runeTr.DOScale(1.2f, _runeFinalShakeTime).SetEase(Ease.Linear).SetAutoKill(true);
            _whiteLight.DOFade(1f, _runeFinalShakeTime).SetEase(Ease.Linear).SetAutoKill(true);

            yield return new WaitForSeconds(_runeFinalShakeTime);

            _whiteLight.color = new Color(1f, 1f, 1f, 0f);

            _runeTr.DOScale(1f, _runeShakeTime / 3f).SetEase(Ease.Linear).SetAutoKill(true);
            _runeTr.DOShakeAnchorPos(_runeShakeTime, _runeShakeStrength, _runeShakeVibrato, 90, false, true, ShakeRandomnessMode.Harmonic).SetAutoKill(true);

            if (_gameManager.GameState.VibrationIsOn)
                Vibration.Vibrate(35);

            yield return new WaitForSeconds(_runeShakeTime / 2f);

            if (isBroken)
            {
                _runeIcon.DOFade(0f, 0.1f);
                _slicedRune.SetActive(true);

                if (_gameManager.GameState.VibrationIsOn)
                    Vibration.Vibrate(100);

                yield return new WaitForSeconds(0.5f);

                foreach (var img in _slicedRuneParts)
                {
                    img.DOFade(0f, _showScreenTime / 2f).SetEase(Ease.Linear).SetAutoKill(true);
                }

                for (int i = 0; i < _backgrounds.Length; i++)
                {
                    _backgrounds[i].DOFade(0f, _showScreenTime / 2f).SetEase(Ease.Linear).SetAutoKill();
                }

                _runeLightTr.DOScale(0f, _showScreenTime / 3f).SetEase(Ease.Linear).SetAutoKill(true);

                yield return new WaitForSeconds(_showScreenTime / 2f);
            }
            else
            {
                _runeTr.DOSizeDelta(_finalRuneSizeDelta, _setRuneTime).SetEase(_setRuneCurve).SetAutoKill(true);

                yield return new WaitForSeconds(0.25f);

                _runeTr.DOMove(_runeSlots[slotIndex].position, _setRuneTime).SetAutoKill(true);

                foreach (var img in _slicedRuneParts)
                {
                    img.DOFade(0f, _setRuneTime / 1.5f).SetEase(Ease.Linear).SetAutoKill(true);
                }

                for (int i = 0; i < _backgrounds.Length; i++)
                {
                    _backgrounds[i].DOFade(0f, _setRuneTime / 1.5f).SetEase(Ease.Linear).SetAutoKill();
                }

                _runeLightTr.DOScale(0f, _setRuneTime / 2.5f).SetEase(Ease.Linear).SetAutoKill(true);

                yield return new WaitForSeconds(_setRuneTime);
            }

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