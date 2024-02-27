using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UIBoosterNotification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _multiplier;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private Image _fade;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _moneyIcon;
        [SerializeField] private Sprite _experienceIcon;

        private GameManager _gameManager;
        private BoosterState _boosterState;
        private BoosterItemData _boosterItemData;
        private float _time;
        private float _setTime;
        private float _activeTimeLeft;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Update()
        {
            _time = _activeTimeLeft + _setTime - Time.realtimeSinceStartup;

            int minutes = (int)_time / 60;
            int seconds = (int)(_time - (minutes * 60));
            _timer.text = minutes.ToString("0#") + ":" + seconds.ToString("00");

            _fade.fillAmount = Mathf.Abs(_time / _boosterItemData.ActiveTime - 1f);

            if(_time <= 0)
            {
                TimeLeft();
            }
        }

        public void Set(BoosterState boosterState, BoosterItemData boosterItemData, BoosterType boosterType)
        {
            _boosterState = boosterState;
            _boosterItemData = boosterItemData;

            _setTime = Time.realtimeSinceStartup;

            float timeDiff = ServerTimeManager.Instance.ServerTime - boosterState.ActiveTimestamp;

            if(timeDiff >= boosterItemData.ActiveTime)
            {
                TimeLeft();
                return;
            }

            switch (boosterType)
            {
                case BoosterType.Money:
                    _icon.sprite = _moneyIcon;
                    _multiplier.text = $"X{boosterItemData.MoneyMultiplier}";
                    break;
                case BoosterType.Experience:
                    _icon.sprite = _experienceIcon;
                    _multiplier.text = $"X{boosterItemData.ExperienceMultiplier}";
                    break;
            }

            _activeTimeLeft = boosterItemData.ActiveTime - timeDiff;
        }

        private void TimeLeft()
        {
            if (_boosterState == null)
                return;

            var currentBooster = _boosterState;

            gameObject.SetActive(false);

            _gameManager.RemoveActiveBooster(currentBooster);
        }

        private void OnDisable()
        {
            _boosterState = null;
        }
    }
}