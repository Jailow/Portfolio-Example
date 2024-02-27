using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
using CaveMiner.Helpers;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UICaveDepthBar : MonoBehaviour
    {
        [SerializeField] private Button _prizeButton;
        [SerializeField] private TextMeshProUGUI _prizeCountText;
        [SerializeField] private TextMeshProUGUI _depthText;
        [SerializeField] private TextMeshProUGUI _multiplierText;
        [SerializeField] private RectTransform _linesParent;
        [SerializeField] private UIDepthBarLine[] _allLines;
        [SerializeField] private Color _defaultLineColor;
        [SerializeField] private Color _maxLineColor;
        [SerializeField] private Color _prizeLineColor;

        private Tween _tween;
        private Tween _prizeTween;
        private GameManager _gameManager;

        private int _centerIndex;
        private int _currentDepth;
        private int _leftLineIndex;
        private float _moveLineLength;
        private Vector3 _defaultLinesPos;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _defaultLinesPos = _linesParent.localPosition;

            _prizeButton.onClick.AddListener(() =>
            {
                _gameManager.AddCavePrize(-1);
            });
        }

        private void OnCaveChanged()
        {
            var caveState = _gameManager.CaveController.CaveState;
            _depthText.text = $"{NumberToString.Convert(caveState.CurrentDepth)} M";
            _multiplierText.text = $"{NumberToString.Convert(caveState.DifficultyMultiplier)} X";

            SetDepth(caveState.CurrentDepth);
        }

        private void OnCaveDifficultyMultiplierChanged()
        {
            var caveState = _gameManager.CaveController.CaveState;
            _multiplierText.text = $"{NumberToString.Convert(caveState.DifficultyMultiplier)} X";
        }

        private void OnPrizeCaveChanged()
        {
            UpdateVisual();
        }

        private void OnBlockDestroyed(BlockData blockData)
        {
            var caveState = _gameManager.CaveController.CaveState;
            _depthText.text = $"{NumberToString.Convert(caveState.CurrentDepth)} M";

            MoveLine();
        }

        private void SetDepth(int depth)
        {
            _currentDepth = depth;

            for(int i = 0; i < _allLines.Length; i++) // —тавим линии обратно в их стандартное положение
            {
                _allLines[i].Tr.SetSiblingIndex(i);
            }

            int value = depth % 5;

            _leftLineIndex = 0;
            _centerIndex = _allLines.Length / 2; 
            for (int i = 0; i < value; i++) // —двигаем их учитыва€ текущую глубину
            {
                _allLines[_leftLineIndex].Tr.SetSiblingIndex(_allLines.Length - 1);
                _leftLineIndex++;
                _centerIndex++;
            }

            UpdateVisual();
        }

        private void UpdateLine(int lineIndex)
        {
            var caveState = _gameManager.CaveController.CaveState;

            int differentCount = (_allLines.Length / 2) - lineIndex; // »ндекс со сдвигом от центра всех индексов
            int finalIndex = _centerIndex - differentCount; // –азница между текущим центральным индексом и его сдвигом
            if (finalIndex < 0)
                finalIndex = _allLines.Length + finalIndex; // ≈сли индекс уходит в минус то берем их с конца листа, замыка€ всю цикличность
            else if (finalIndex >= _allLines.Length)
                finalIndex = Mathf.Abs(finalIndex - _allLines.Length); // ≈сли индекс уходит в перебор то берем его с начала листа, замыкаю всю цикличность

            int lineDepth = _currentDepth - differentCount; // √лубина текущей обновл€емой линии

            if (lineDepth == caveState.MaxDepth && lineDepth > 0) // ≈сли глубина это рекорд
            {
                _allLines[finalIndex].MaxObj.SetActive(true);
                _allLines[finalIndex].Icon?.gameObject.SetActive(false);
                _allLines[finalIndex].Line.color = _maxLineColor;
            }
            else
            {
                _allLines[finalIndex].MaxObj.SetActive(false);

                if (lineDepth <= _currentDepth) // ≈сли глубина линии меньше чем текуща€ глубина
                {
                    if (lineDepth % _gameManager.GameData.PriceEveryDepth == 0 && lineDepth > 0)
                    {
                        _allLines[finalIndex].Icon?.gameObject.SetActive(false);
                        _allLines[finalIndex].Line.color = _prizeLineColor;
                    }
                    else
                    {
                        _allLines[finalIndex].Icon?.gameObject.SetActive(false);
                        _allLines[finalIndex].Line.color = _defaultLineColor;
                    }
                }
                else // ≈сли глубина линии больше чем текуща€ глубина
                {
                    if (lineDepth % _gameManager.GameData.PriceEveryDepth == 0)
                    {
                        _allLines[finalIndex].Icon?.gameObject.SetActive(true);
                        _allLines[finalIndex].Line.color = _prizeLineColor;
                    }
                    else
                    {
                        _allLines[finalIndex].Icon?.gameObject.SetActive(false);
                        _allLines[finalIndex].Line.color = _defaultLineColor;
                    }
                }
            }
        }

        private void MoveLine()
        {
            _linesParent.localPosition = _defaultLinesPos - (CachedVector3.Left * _moveLineLength); // двигаем все линии вперед на одну
            _allLines[_leftLineIndex].Tr.SetSiblingIndex(_allLines.Length - 1); // ѕереставл€ем самую левую линию в правую сторону, в самый конец

            _leftLineIndex++;
            if (_leftLineIndex >= _allLines.Length)
                _leftLineIndex = 0;

            _centerIndex++;
            if (_centerIndex >= _allLines.Length)
                _centerIndex = 0;

            _currentDepth++;

            UpdateLine(_allLines.Length - 1);
            UpdateLine(_allLines.Length / 2);

            _tween?.Kill();
            _tween = _linesParent.DOLocalMoveX(_linesParent.localPosition.x - _moveLineLength, 0.15f); // јнимированно двигаем все линии влево
        }

        public async void Show()
        {
            gameObject.SetActive(true);

            OnCaveChanged();

            await Task.Delay(5);

            int secondIndex = _leftLineIndex + 1;
            if (secondIndex >= _allLines.Length)
                secondIndex = 0;

            _moveLineLength = Mathf.Abs(_allLines[_leftLineIndex].Tr.localPosition.x - _allLines[secondIndex].Tr.localPosition.x);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateVisual()
        {
            var caveState = _gameManager.CaveController.CaveState;

            for (int i = 0; i < _allLines.Length; i++)
                UpdateLine(i);

            _prizeCountText.text = caveState.PrizeCount.ToString();

            bool showPrize = caveState.PrizeCount > 0;
            _prizeButton.gameObject.SetActive(showPrize);
            _prizeTween?.Kill();

            if(showPrize)
                _prizeTween = DOTween.Sequence().Append(_prizeButton.transform.DOScale(1.15f, 0.35f).SetEase(Ease.OutSine)).Append(_prizeButton.transform.DOScale(0.95f, 0.35f).SetEase(Ease.OutSine)).SetLoops(-1);
        }

        private void OnEnable()
        {
            _gameManager.onCaveChanged += OnCaveChanged;
            _gameManager.onCavePrizeChanged += OnPrizeCaveChanged;
            _gameManager.CaveController.onBlockDestroyed += OnBlockDestroyed;
            _gameManager.onCaveDifficultyMultiplierChanged += OnCaveDifficultyMultiplierChanged;

            UpdateVisual();
        }

        private void OnDisable()
        {
            _gameManager.onCaveChanged -= OnCaveChanged;
            _gameManager.onCavePrizeChanged -= OnPrizeCaveChanged;
            _gameManager.CaveController.onBlockDestroyed -= OnBlockDestroyed;
            _gameManager.onCaveDifficultyMultiplierChanged -= OnCaveDifficultyMultiplierChanged;
        }
    }
}