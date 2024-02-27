using System.Collections.Generic;
using UnityEngine;

namespace CaveMiner.UI
{
    public class UIPickaxeWealthUpgradeTab : UIPickaxeUpgradeTabBase
    {
        public override void Init(GameManager gameManager, UIManager uiManager)
        {
            base.Init(gameManager, uiManager);

            gameManager.Pickaxe.onWealthChanged += UpdateStats;

            UpdateStats();
        }

        protected override void OnMoneyChanged(int count)
        {
            var playerState = _gameManager.GameState.PlayerState;
            if (playerState.PickaxeState.WealthLevel >= _maxLevel)
            {
                _disabledButtonAlpha.Interactable = false;
                _priceText.text = I2.Loc.LocalizationManager.GetTranslation(MAX_LEVEL_KEY);
                return;
            }

            base.OnMoneyChanged(count);
        }

        protected override void Upgrade()
        {
            base.Upgrade();

            if (_gameManager.PickaxeState.WealthLevel >= _maxLevel)
                return;

            var playerState = _gameManager.GameState.PlayerState;

            if (playerState.Money < _currentPrice)
                return;

            #region UpgradePickaxeWealthEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("CaveLevel", playerState.CaveLevel);
            properties.Add("RebirthCount", playerState.Stats.RebirthCount);
            properties.Add("PickaxeSpeed", playerState.PickaxeState.DamageLevel);
            properties.Add("PickaxeWealth", playerState.PickaxeState.WealthLevel);
            properties.Add("PickaxeExperience", playerState.PickaxeState.ExperienceLevel);
            properties.Add("PickaxeAutoClick", playerState.PickaxeState.AutoClickLevel);
            properties.Add("Money", playerState.Money);

            AmplitudeManager.Instance.Event(AnalyticEventKey.UPGRADE_PICKAXE_WEALTH, properties);
            #endregion

            _gameManager.AddMoney(-_currentPrice);
            _gameManager.Pickaxe.AddWealth(1);

            AchievementManager.Instance.CheckPickaxeWealthLevelCondition(playerState.PickaxeState.WealthLevel);
        }

        private void UpdateStats()
        {
            var pickaxeState = _gameManager.PickaxeState;
            var playerState = _gameManager.GameState.PlayerState;

            _levelText.text = pickaxeState.WealthLevel.ToString();

            if (playerState.PickaxeState.WealthLevel >= _maxLevel)
            {
                _disabledButtonAlpha.Interactable = false;
                _priceText.text = I2.Loc.LocalizationManager.GetTranslation(MAX_LEVEL_KEY);
            }
            else
            {
                _currentPrice = Mathf.RoundToInt(_basePrice * Mathf.Pow(_priceMultiplierPerLevel, pickaxeState.WealthLevel - 1));
                _priceText.text = Helpers.NumberToString.Convert(_currentPrice);
            }

            OnMoneyChanged(playerState.Money);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateStats();
        }
    }
}