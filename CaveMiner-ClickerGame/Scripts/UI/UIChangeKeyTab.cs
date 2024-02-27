using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CaveMiner.UI
{
    public class UIChangeKeyTab : MonoBehaviour
    {
        [SerializeField] private Button _changeBtn;
        [SerializeField] private int _oldKeyCount;
        [SerializeField] private RarityType _oldKeyType;
        [SerializeField] private int _newKeyCount;
        [SerializeField] private RarityType _newKeyType;
        [SerializeField] private Color _defaultTextColor;
        [SerializeField] private Color _redTextColor;
        [SerializeField] private TextMeshProUGUI _oldKeyText;

        private GameManager _gameManager;
        private UIDisabledButtonAlpha _disabledButton;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;

            _disabledButton = _changeBtn.GetComponent<UIDisabledButtonAlpha>();
            _disabledButton.Init();

            _changeBtn.onClick.AddListener(() =>
            {
                var playerState = _gameManager.GameState.PlayerState;
                playerState.Stats.ExchangedKeyCount++;

                AchievementManager.Instance.CheckExchangeKeyCountCondition(playerState.Stats.ExchangedKeyCount);

                uiManager.ButtonClickSound();
                _gameManager.AddKey(_oldKeyType, -_oldKeyCount);
                _gameManager.AddKey(_newKeyType, _newKeyCount);

                Dictionary<string, object> properties = new Dictionary<string, object>();

                properties.Add("From", _oldKeyCount.ToString());
                properties.Add("To", _newKeyType.ToString());

                AmplitudeManager.Instance.Event(AnalyticEventKey.CHANGE_KEY, properties);
            });
        }

        private void UpdateVisual()
        {
            bool haveKeys  = _gameManager.GetPlayerKeyCount(_oldKeyType) >= _oldKeyCount;
            
            _oldKeyText.color = haveKeys ? _defaultTextColor : _redTextColor;
            _disabledButton.Interactable = haveKeys;
        }

        private void OnEnable()
        {
            _gameManager.onKeyChanged += UpdateVisual;

            UpdateVisual();
        }

        private void OnDisable()
        {
            _gameManager.onKeyChanged -= UpdateVisual;
        }
    }
}