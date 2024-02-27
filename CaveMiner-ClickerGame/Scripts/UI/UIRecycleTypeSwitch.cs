using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRecycleTypeSwitch : MonoBehaviour
    {
        [SerializeField] private Vector2 _firstPos;
        [SerializeField] private Vector2 _secondPos;
        [SerializeField] private RectTransform _rectTr;

        private Tween _tween;
        private Button _btn;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();

                var recycleState = _gameManager.GameState.PlayerState.RecyclingState;
                recycleState.RecycleType = recycleState.RecycleType == 0 ? 1 : 0;

                _tween?.Kill();
                _tween = _rectTr.DOAnchorPos(recycleState.RecycleType == 0 ? _firstPos : _secondPos, 0.25f);
            });

            var recycleState = _gameManager.GameState.PlayerState.RecyclingState;
            Set(recycleState.RecycleType == 1);
        }

        public void Set(bool active)
        {
            _rectTr.anchoredPosition = active ? _secondPos : _firstPos;
        }


    }
}