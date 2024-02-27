using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace CaveMiner.UI
{
    public class UIReviewPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _starsPanel;
        [SerializeField] private GameObject _reviewPanel;
        [SerializeField] private Button _noBtn;
        [SerializeField] private Button _yesBtn;
        [SerializeField] private Button[] _starsButton;
        [SerializeField] private Sprite _selectedStarSprite;

        public bool IsOpened { get; private set; }

        private GameManager _gameManager;
        private UIManager _uiManager;
        private const string GOOGLE_URL = "https://play.google.com/store/apps/details?id=com.TechJungle.CaveMinerClickerGame";

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _noBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Hide();
            });

            _yesBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();

                Application.OpenURL(GOOGLE_URL);

                Invoke(nameof(IsRated), 1f);

                Hide();
            });

            for (int i = 0; i < _starsButton.Length; i++)
            {
                int index = (int)(i);
                _starsButton[i].onClick.AddListener(() =>
                {
                    OnSelectStar(index);
                });
            }
        }

        private void IsRated()
        {
            _gameManager.GameState.IsRated = true;
            _gameManager.AddMoney(1000);
        }

        private void OnSelectStar(int index)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("StarCount", index + 1);
            AmplitudeManager.Instance.Event(AnalyticEventKey.REVIEW_STARS, properties);

            StartCoroutine(StarAnimation(index));
        }

        private IEnumerator StarAnimation(int index)
        {
            foreach (var btn in _starsButton)
                btn.interactable = false;

            for(int i = 0; i < _starsButton.Length; i++)
            {
                yield return new WaitForSeconds(0.1f);

                if (i > index)
                    break;

                DOTween.Sequence().Append(_starsButton[i].transform.DOScale(1.1f, 0.1f)).Append(_starsButton[i].transform.DOScale(1f, 0.1f));

                yield return new WaitForSeconds(0.1f);

                _starsButton[i].image.sprite = _selectedStarSprite;

            }

            yield return new WaitForSeconds(1f);

            if (index <= 3)
            {
                _gameManager.GameState.IsRated = true;

                Hide();
                yield break;
            }

            _starsPanel.SetActive(false);
            _reviewPanel.SetActive(true);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            _starsPanel.SetActive(true);
            _reviewPanel.SetActive(false);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
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