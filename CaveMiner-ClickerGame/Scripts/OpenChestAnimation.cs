using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CaveMiner
{
    public class OpenChestAnimation : MonoBehaviour
    {
        [Header("Backgrounds")]
        [SerializeField] private Image[] _backgrounds;
        [SerializeField] private float _showScreenTime;
        [SerializeField] private AnimationCurve _showScreenCurve;
        [SerializeField] private RectTransform _lightTr;
        [SerializeField] private RectTransform _lightRayTr;
        [SerializeField] private float _showLightTime;
        [SerializeField] private Image _chestIcon;
        [SerializeField] private float _showChestTime;
        [SerializeField] private float _rotateLightRayTime;
        [SerializeField] private AnimationCurve _showChestCurve;
        [SerializeField] private int _chestShakeCount;
        [SerializeField] private float _chestShakeTime;
        [SerializeField] private float _chestShakeStregth;
        [SerializeField] private int _chestShakeVibrato;
        [SerializeField] private int _itemsCount;
        [SerializeField] private float _itemRandomizePosition;
        [SerializeField] private float _itemShowTime;
        [SerializeField] private float _hideItemTime;
        [SerializeField] private AnimationCurve _showItemCurve;
        [SerializeField] private AnimationCurve _itemFinalMoveCurve;
        [SerializeField] private Image _itemPrefab;
        [SerializeField] private Transform _itemParent;
        [SerializeField] private Transform _dropItemFinalPosition;

        private Color[] _backgroundColors;
        private Tween _rotateRayTween;
        private GameManager _gameManager;
        private Image[] _itemPrefabs;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _itemPrefabs = new Image[_itemsCount];
            for(int i = 0; i < _itemPrefabs.Length; i++)
            {
                _itemPrefabs[i] = Instantiate(_itemPrefab, _itemParent, false);
            }

            _backgroundColors = new Color[_backgrounds.Length];
            for (int i = 0; i < _backgroundColors.Length; i++)
            {
                _backgroundColors[i] = _backgrounds[i].color;
            }
        }

        public void Show(Sprite chestIcon, ItemData[] dropItems, ItemData finalItem, Action onCompleted)
        {
            gameObject.SetActive(true);

            StartCoroutine(Animation(chestIcon, dropItems, finalItem, onCompleted));
        }

        private void ResetAll()
        {
            foreach (var img in _backgrounds)
            {
                img.color = Color.clear;
            }

            _itemPrefab.rectTransform.anchoredPosition = Helpers.CachedVector3.Zero;
            _itemPrefab.rectTransform.localScale = Helpers.CachedVector3.Zero;
            _itemPrefab.color = Color.white;

            foreach(var item in _itemPrefabs)
            {
                item.rectTransform.anchoredPosition = Helpers.CachedVector2.Zero;
                item.rectTransform.localScale = Helpers.CachedVector3.Zero;
                item.color = Color.white;
            }

            _chestIcon.color = Color.white;
            _chestIcon.rectTransform.localScale = Helpers.CachedVector3.Zero;
            _lightRayTr.localScale = Helpers.CachedVector3.Zero;
            _lightTr.localScale = Helpers.CachedVector3.Zero;
        }

        private IEnumerator Animation(Sprite chestIcon, ItemData[] dropItems, ItemData finalItem, Action onCompleted)
        {
            if(_gameManager.GameState.PlayerState.CaveStates.Count >= 2)
                CleverAdsSolutionsManager.Instance.EnableTimer = false;

            ResetAll();

            _chestIcon.sprite = chestIcon;

            for (int i = 0; i < _backgrounds.Length; i++)
            {
                _backgrounds[i].DOColor(_backgroundColors[i], _showScreenTime).SetEase(_showScreenCurve).SetAutoKill();
            }

            yield return new WaitForSeconds(_showScreenTime);

            _lightTr.DOScale(1f, _showLightTime).SetEase(Ease.Linear).SetAutoKill(true);

            yield return new WaitForSeconds(_showLightTime / 2f);

            _chestIcon.rectTransform.DOScale(1f, _showChestTime).SetEase(_showChestCurve).SetAutoKill(true);
            _lightRayTr.DOScale(1f, _showChestTime).SetEase(Ease.Linear).SetAutoKill(true);

            _rotateRayTween = _lightRayTr.DORotate(Helpers.CachedVector3.Forward * 360f, _rotateLightRayTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);

            yield return new WaitForSeconds(_showChestTime);

            for (int i = 0; i < _chestShakeCount; i++)
            {
                if (_gameManager.GameState.VibrationIsOn)
                    Vibration.Vibrate(35);

                _chestIcon.rectTransform.DOShakeScale(_chestShakeTime, _chestShakeStregth, _chestShakeVibrato, 40, true, ShakeRandomnessMode.Harmonic);

                if(i < _chestShakeCount -1)
                    yield return new WaitForSeconds(_chestShakeTime * 1.25f);
            }

            _chestIcon.DOFade(0f, _showChestTime).SetAutoKill(true);
            _lightRayTr.DOScale(0f, _showChestTime).SetEase(Ease.Linear).SetAutoKill(true);

            yield return new WaitForSeconds(_showChestTime * 0.4f);

            _lightTr.DOScale(0f, _showChestTime).SetEase(Ease.Linear).SetAutoKill(true);

            if (_gameManager.GameState.VibrationIsOn)
                Vibration.Vibrate(20);

            foreach (var item in _itemPrefabs)
            {
                item.sprite = dropItems[Random.Range(0, dropItems.Length)].Icon;
                item.rectTransform.DOScale(1f, _itemShowTime / 3f).SetEase(Ease.Linear).SetAutoKill(true);
                item.rectTransform.DOAnchorPos(new Vector2(Random.Range(-_itemRandomizePosition, _itemRandomizePosition), Random.Range(-_itemRandomizePosition, _itemRandomizePosition)), _itemShowTime).SetEase(_showItemCurve).SetAutoKill(true);
            }

            var itm = _gameManager.Items.FirstOrDefault(e => e.Id == finalItem.Id);
            _itemPrefab.sprite = itm.Icon;
            _itemPrefab.rectTransform.DOScale(1f, _itemShowTime / 3f).SetEase(Ease.Linear).SetAutoKill(true);
            _itemPrefab.rectTransform.DOAnchorPos(new Vector2(Random.Range(-_itemRandomizePosition, _itemRandomizePosition), Random.Range(-_itemRandomizePosition, _itemRandomizePosition)), _itemShowTime).SetEase(_showItemCurve).SetAutoKill(true);

            yield return new WaitForSeconds(_itemShowTime);

            //foreach (var item in _itemPrefabs)
            //{
            //    item.rectTransform.DOShakeAnchorPos(0.8f, 6f, 4, 90, false, true, ShakeRandomnessMode.Harmonic);
            //}

            //_itemPrefab.rectTransform.DOShakeAnchorPos(0.8f, 6f, 4, 90, false, true, ShakeRandomnessMode.Harmonic);

            //yield return new WaitForSeconds(0.8f);

            foreach (var item in _itemPrefabs)
            {
                item.rectTransform.DOScale(1.5f, _hideItemTime).SetEase(Ease.Linear).SetAutoKill(true);
                item.DOFade(0f, _hideItemTime).SetEase(Ease.Linear).SetAutoKill(true);
                item.rectTransform.DOMove(_dropItemFinalPosition.position, _hideItemTime).SetEase(_itemFinalMoveCurve).SetAutoKill(true);
                yield return new WaitForSeconds(_hideItemTime / 3f);
            }

            _itemPrefab.rectTransform.DOScale(1.75f, _hideItemTime).SetEase(Ease.Linear).SetAutoKill(true);
            _itemPrefab.rectTransform.DOMove(_dropItemFinalPosition.position, _hideItemTime).SetEase(_itemFinalMoveCurve).SetAutoKill(true);

            yield return new WaitForSeconds(_hideItemTime * 2f);

            _itemPrefab.DOFade(0f, _hideItemTime).SetEase(Ease.Linear).SetAutoKill(true);

            yield return new WaitForSeconds(_hideItemTime / 2f);

            for (int i = 0; i < _backgrounds.Length; i++)
            {
                _backgrounds[i].DOFade(0f, _showScreenTime / 2f).SetEase(Ease.Linear).SetAutoKill();
            }

            _lightTr.DOScale(0f, _showScreenTime / 3f).SetEase(Ease.Linear).SetAutoKill(true);

            yield return new WaitForSeconds(_showScreenTime / 2f);

            _rotateRayTween.Kill();
            _rotateRayTween = null;

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