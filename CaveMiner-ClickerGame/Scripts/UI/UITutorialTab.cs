using DG.Tweening;
using UnityEngine;

namespace CaveMiner.UI
{
    public class UITutorialTab : MonoBehaviour
    {
        [SerializeField] private string _tutorialId;
        [SerializeField] private Vector2 _defaultPos;
        [SerializeField] private Vector2 _hiddenPos;
        [SerializeField] private float _animationTime;

        private GameManager _gameManager;
        private RectTransform _tr;

        private void Awake()
        {
            if (string.IsNullOrEmpty(_tutorialId))
                return;

            _gameManager = FindObjectOfType<GameManager>();

            _tr = GetComponent<RectTransform>();

            bool tutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains(_tutorialId);
            _tr.anchoredPosition = tutorialCompleted ? _defaultPos : _hiddenPos;
        }

        public void ShowAnimation()
        {
            _tr.DOAnchorPos(_defaultPos, _animationTime).SetEase(Ease.InOutSine);
        }
    }
}