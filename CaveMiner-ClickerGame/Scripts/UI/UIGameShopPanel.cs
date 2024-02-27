using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaveMiner.Helpers;
using System.Collections.Generic;

namespace CaveMiner.UI
{
    public class UIGameShopPanel : MonoBehaviour
    {
        [SerializeField] private UIShopItemTab[] _shopItemTabs;
        [SerializeField] private Button _updateItemsButton;
        [SerializeField] private TextMeshProUGUI _updateItemsPriceText;
        [SerializeField] private TextMeshProUGUI _updateTimerText;

        private UIDisabledButtonAlpha _disabledButtonAlpha;
        private GameManager _gameManager;

        private const string UPDATE_ITEMS_KEY = "update_game_shop";
        private const string YES_KEY = "yes";
        private const string NO_KEY = "no";

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;

            _disabledButtonAlpha = _updateItemsButton.GetComponent<UIDisabledButtonAlpha>();
            _disabledButtonAlpha.Init();

            foreach (var tab in _shopItemTabs)
                tab.Init(gameManager, uiManager);

            _updateItemsPriceText.text = _gameManager.GameData.PriceToUpdateGameShop.ToString();

            string[] infoPanelArgs = new string[] { _gameManager.GameData.PriceToUpdateGameShop.ToString() };
            _updateItemsButton.onClick.AddListener(() =>
            {
                var playerState = _gameManager.GameState.PlayerState;
                playerState.Stats.GameShopUpdateItemsCount++;

                AchievementManager.Instance.CheckUpdateGameShopCountCondition(playerState.Stats.GameShopUpdateItemsCount);

                uiManager.ButtonClickSound();
                uiManager.InfoPanel.Show(UPDATE_ITEMS_KEY, YES_KEY, NO_KEY, OnUpdateItems, null, infoPanelArgs);
            });

            var playerState = _gameManager.GameState.PlayerState;

            for (int i = 0; i < 3; i++)
            {
                _shopItemTabs[i].SetItem(playerState.GameShop.Items[i]);
            }
        }

        private void Update()
        {
            var playerState = _gameManager.GameState.PlayerState;
            int serverTime = _gameManager.GameData.AutoUpdateGameShopTime - ((int)ServerTimeManager.Instance.ServerTime - playerState.GameShop.LastUpdateTimestamp);
            _updateTimerText.text = Timestamp.ToHoursTimer(serverTime);

            TryAutoUpdate();
        }

        public void OnUpdateItems()
        {
            var playerState = _gameManager.GameState.PlayerState;

            if (playerState.Money < _gameManager.GameData.PriceToUpdateGameShop)
                return;

            #region UpdateGameShopEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("Money", playerState.Money);

            AmplitudeManager.Instance.Event(AnalyticEventKey.UPDATE_GAME_SHOP, properties);
            #endregion

            _gameManager.AddMoney(-_gameManager.GameData.PriceToUpdateGameShop);
            _gameManager.UpdateGameShop();

            for(int i = 0; i < 3; i++)
            {
                _shopItemTabs[i].SetItem(playerState.GameShop.Items[i]);
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            var playerState = _gameManager.GameState.PlayerState;

            _disabledButtonAlpha.Interactable = playerState.Money >= _gameManager.GameData.PriceToUpdateGameShop;

            foreach (var tab in _shopItemTabs)
                tab.UpdateTab();
        }

        private void OnMoneyChanged(int count)
        {
            UpdateVisual();
        }

        private void TryAutoUpdate()
        {
            var playerState = _gameManager.GameState.PlayerState;
            int timeToUpdateShop = _gameManager.GameData.AutoUpdateGameShopTime;

            int currentTime = (int)ServerTimeManager.Instance.ServerTime;
            int differentTime = currentTime - playerState.GameShop.LastUpdateTimestamp;

            if (differentTime >= timeToUpdateShop)
            {
                int timeToNextUpdate = timeToUpdateShop - (differentTime % timeToUpdateShop);
                int lastUpdateTime = currentTime - (timeToUpdateShop - timeToNextUpdate);

                _gameManager.UpdateGameShop(lastUpdateTime);

                for (int i = 0; i < 3; i++)
                {
                    _shopItemTabs[i].SetItem(playerState.GameShop.Items[i]);
                }

                UpdateVisual();
            }
        }

        private void OnEnable()
        {
            _gameManager.onMoneyChanged += OnMoneyChanged;

            UpdateVisual();

            TryAutoUpdate();
        }

        private void OnDisable()
        {
            _gameManager.onMoneyChanged -= OnMoneyChanged;
        }
    }
}