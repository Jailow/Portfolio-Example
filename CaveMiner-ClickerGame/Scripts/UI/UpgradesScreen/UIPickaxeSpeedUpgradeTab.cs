using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CaveMiner.UI
{
    public class UIPickaxeSpeedUpgradeTab : UIPickaxeUpgradeTabBase
    {
        [SerializeField] private AnimationCurve _priceMultiplierCurve;

        public override void Init(GameManager gameManager, UIManager uiManager)
        {
            base.Init(gameManager, uiManager);

            gameManager.Pickaxe.onSpeedChanged += UpdateStats;

            UpdateStats();
        }

        protected override void OnMoneyChanged(int count)
        {
            var playerState = _gameManager.GameState.PlayerState;
            if (playerState.PickaxeState.DamageLevel >= _maxLevel)
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

            if (_gameManager.PickaxeState.DamageLevel >= _maxLevel)
                return;

            var playerState = _gameManager.GameState.PlayerState;;

            if (playerState.Money < _currentPrice)
                return;

            #region UpgradePickaxeSpeedEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("CaveLevel", playerState.CaveLevel);
            properties.Add("RebirthCount", playerState.Stats.RebirthCount);
            properties.Add("PickaxeSpeed", playerState.PickaxeState.DamageLevel);
            properties.Add("PickaxeWealth", playerState.PickaxeState.WealthLevel);
            properties.Add("PickaxeExperience", playerState.PickaxeState.ExperienceLevel);
            properties.Add("PickaxeAutoClick", playerState.PickaxeState.AutoClickLevel);
            properties.Add("Money", playerState.Money);

            AmplitudeManager.Instance.Event(AnalyticEventKey.UPGRADE_PICKAXE_SPEED, properties);
            #endregion

            _gameManager.AddMoney(-_currentPrice);
            _gameManager.Pickaxe.AddDamage(1);

            AchievementManager.Instance.CheckPickaxeDamageLevelCondition(playerState.PickaxeState.DamageLevel);
        }

        private void UpdateStats()
        {
            var pickaxeState = _gameManager.PickaxeState;
            var playerState = _gameManager.GameState.PlayerState;

            _levelText.text = pickaxeState.DamageLevel.ToString();
            if (playerState.PickaxeState.DamageLevel >= _maxLevel)
            {
                _disabledButtonAlpha.Interactable = false;
                _priceText.text = I2.Loc.LocalizationManager.GetTranslation(MAX_LEVEL_KEY);
            }
            else
            {
                float priceMultiplier = _priceMultiplierPerLevel * _priceMultiplierCurve.Evaluate((float)playerState.PickaxeState.DamageLevel / _maxLevel);
                _currentPrice = Mathf.RoundToInt(_basePrice * Mathf.Pow(priceMultiplier, pickaxeState.DamageLevel - 1));
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