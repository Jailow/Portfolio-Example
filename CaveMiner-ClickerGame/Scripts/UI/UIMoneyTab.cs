using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UIMoneyTab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Button _plusBtn;

        private GameManager _gameManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;

            _gameManager.onMoneyChanged += OnMoneyChanged;

            _plusBtn.onClick.AddListener(() =>
            {
                uiManager.NavigationPanel.SelectButton(NavigationButtonType.Shop);
                uiManager.ShopScreen.SelectCategory(2);
            });

            UpdateTab();
        }

        private void OnMoneyChanged(int count)
        {
            _countText.text = Helpers.NumberToString.Convert(count);
        }

        private void UpdateTab()
        {
            var playerState = _gameManager.GameState.PlayerState;
            OnMoneyChanged(playerState.Money);
        }

        private void OnEnable()
        {
            if (_gameManager != null)
            {
                UpdateTab();
            }
        }
    }
}