using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaveMiner.Helpers;

namespace CaveMiner.UI
{
    public class UICombineRunePanel : MonoBehaviour
    {
        [SerializeField] private int _priceToCombine;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private Button _combineButton;
        [SerializeField] private List<UICombineRuneSlot> _slots;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Color _defaultTextColor;
        [SerializeField] private Color _warningTextColor;

        public bool IsOpened { get; private set; }

        private UIDisabledButtonAlpha _disabledButton;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _disabledButton = _combineButton.GetComponent<UIDisabledButtonAlpha>();
            _disabledButton.Init();

            foreach (var slot in _slots)
                slot.Init(gameManager, uiManager, UpdateVisual);

            _backgroundCloseButton.onClick.AddListener(Hide);
            _closeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Hide();
            });

            _combineButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                Combine();
            });
        }

        private void UpdateVisual()
        {
            var playerState = _gameManager.GameState.PlayerState;

            bool haveMoney = playerState.Money >= _priceToCombine;
            bool runePlaced = _slots.All(e => e.ItemState != null);
            
            _moneyText.text = $"{NumberToString.Convert(playerState.Money)}/{NumberToString.Convert(_priceToCombine)}";
            _moneyText.color = haveMoney ? _defaultTextColor : _warningTextColor;

            if(haveMoney && runePlaced)
            {
                bool sameRunes = _slots.All(e => e.ItemState.Id == _slots[0].ItemState.Id);
                bool sameRuneChance = _slots.All(e => e.ItemState.Id == _slots[0].ItemState.Id && e.ItemState.CustomValue == _slots[0].ItemState.CustomValue);

                if (sameRuneChance)
                {
                    _disabledButton.Interactable = _slots[0].ItemState.Count >= 2;
                }
                else
                {
                    _disabledButton.Interactable = sameRunes;
                }
            }
            else
            {
                _disabledButton.Interactable = false;
            }
        }

        private void Combine()
        {
            var playerState = _gameManager.GameState.PlayerState;
            playerState.Stats.CombineRuneCount++;

            AchievementManager.Instance.CheckCombineRuneCountCondition(playerState.Stats.CombineRuneCount);

            int newChance = 0;
            string itemId = _slots[0].ItemState.Id;

            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].ItemState.Count--;
                newChance += int.Parse(_slots[i].ItemState.CustomValue.ToString().TrimEnd('%'));

                if (_slots[i].ItemState.Count <= 0)
                    playerState.Items.Remove(_slots[i].ItemState);

                _slots[i].ResetSlot();
            }

            newChance = Mathf.Clamp(newChance, 0, 100);

            _gameManager.AddItem(itemId, 1, $"{newChance}%");
            _gameManager.AddMoney(-_priceToCombine);

            var itemData = _gameManager.Items.FirstOrDefault(e => e.Id == itemId);
            _uiManager.CombineRuneAnimation.Show(itemData.Icon, ResetPanel);

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("RuneID", itemId);

            AmplitudeManager.Instance.Event(AnalyticEventKey.COMBINE_RUNES, properties);
        }

        private void ResetPanel()
        {
            foreach (var slot in _slots)
                slot.ResetSlot();

            UpdateVisual();
        }

        private void OnMoneyChanged(int count)
        {
            UpdateVisual();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            UpdateVisual();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _gameManager.onMoneyChanged += OnMoneyChanged;

            ResetPanel();

            IsOpened = true;
        }

        private void OnDisable()
        {
            _gameManager.onMoneyChanged -= OnMoneyChanged;

            IsOpened = false;
        }
    }
}