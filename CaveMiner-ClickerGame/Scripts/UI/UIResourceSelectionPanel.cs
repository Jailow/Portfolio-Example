using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Linq;

namespace CaveMiner.UI
{
    public enum ResourceSelectionPanelType
    {
        Fullscreen,
        Panel,
    }

    public class UIResourceSelectionPanel : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _leftArrow;
        [SerializeField] private Button _rightArrow;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _caveButtonPrefab;
        [SerializeField] private Transform _resourceParent;
        [SerializeField] private UISelectResourceItemTab _selectResourceItemTab;

        public bool IsOpened { get; private set; }

        private int _activeCaveCount;
        private Action<ResourceItemState, int, Vector3> _onItemTake;
        private Vector2 _startTouchPos;
        private int _currentCaveIndex;
        private List<Button> _allCaveButtons;
        private List<UISelectResourceItemTab> _resourceTabs;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));

            _leftArrow.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                SelectCave(--_currentCaveIndex, true);
            });

            _rightArrow.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                SelectCave(++_currentCaveIndex, true);
            });

            _allCaveButtons = new List<Button>();
            _resourceTabs = new List<UISelectResourceItemTab>();

            for(int i = 0; i < _gameManager.Caves.Length; i++)
            {
                var caveButton = Instantiate(_caveButtonPrefab, _scrollRect.content);
                int copyNum = (int)i;
                caveButton.onClick.AddListener(() => SelectCave(copyNum, true));
                caveButton.image.sprite = _gameManager.Caves[i].Icon;
                _allCaveButtons.Add(caveButton);
            }

            _caveButtonPrefab.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.touchCount <= 0)
                return;

            var touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                _startTouchPos = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if(Vector2.Distance(_startTouchPos, touch.position) > 10)
                    SelectNearCave();
            }
        }

        private void SelectNearCave()
        {
            float partValue = 1f / (_activeCaveCount - 1);

            int finalCaveIndex = 0;
            float minLenght = float.MaxValue;

            for(int i = 0; i < _activeCaveCount; i++)
            {
                var length = Mathf.Abs((i * partValue) - _scrollRect.horizontalNormalizedPosition);
                if (length <= minLenght)
                {
                    finalCaveIndex = i;
                    minLenght = length;
                }
            }

            SelectCave(finalCaveIndex, true);
        }

        private void SelectCave(int index, bool useAnimation)
        {
            index = Mathf.Clamp(index, 0, _activeCaveCount);

            _uiManager.ButtonClickSound();

            _currentCaveIndex = index;

            if (useAnimation)
            {
                StopAllCoroutines();
                StartCoroutine(ScrollRectAnimation(index * (1f / (_activeCaveCount - 1))));
            }

            foreach (var item in _resourceTabs)
            {
                item.gameObject.SetActive(false);
            }

            var playerState = _gameManager.GameState.PlayerState;
            var blockDatas = _gameManager.Caves[_currentCaveIndex].BlocksDatas;

            int resourceIndex = 0;
            foreach (var itemState in playerState.ResourceItems)
            {
                var item = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == itemState.Id);

                if (item == null || item.CaveData.Id != _gameManager.Caves[_currentCaveIndex].Id)
                    continue;

                if (resourceIndex >= _resourceTabs.Count)
                {
                    var newItem = Instantiate(_selectResourceItemTab, _resourceParent);
                    newItem.Init(_gameManager, _uiManager, OnTakeItem);
                    _resourceTabs.Add(newItem);
                }

                _resourceTabs[resourceIndex].gameObject.SetActive(true);
                _resourceTabs[resourceIndex].Set(itemState);

                resourceIndex++;
            }
        }

        private void OnTakeItem(ResourceItemState itemState, int count, Vector3 pos)
        {
            _onItemTake?.Invoke(itemState, count, pos);
        }

        private IEnumerator ScrollRectAnimation(float finalValue)
        {
            float prevValue = _scrollRect.horizontalNormalizedPosition;
            float time = 0;
            while(time <= 1f)
            {
                time += Time.deltaTime * 5f;
                _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(prevValue, finalValue, time);

                yield return null;
            }

            _scrollRect.horizontalNormalizedPosition = finalValue;
        }

        public void Show(Action<ResourceItemState, int, Vector3> onItemTake, bool isFullscreen)
        {
            _closeButton.gameObject.SetActive(isFullscreen);

            _onItemTake = onItemTake;

            SelectCave(0, false);
            _scrollRect.horizontalNormalizedPosition = 0f;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IsOpened = true;

            var rectTr = _uiManager.Canvas.GetComponent<RectTransform>();
            int padding = (int)((rectTr.sizeDelta.x / 2f) - 201);
            _horizontalLayoutGroup.padding.left = padding;
            _horizontalLayoutGroup.padding.right = padding;

            _activeCaveCount = 0;
            var playerState = _gameManager.GameState.PlayerState;
            for(int i = 0; i < _allCaveButtons.Count; i++)
            {
                if (i >= playerState.CaveStates.Count)
                {
                    _allCaveButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    _allCaveButtons[i].gameObject.SetActive(playerState.CaveStates[i].Level > 0);
                    _activeCaveCount++;
                }
            }
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}