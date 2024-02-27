using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CaveMiner
{
    public class Pickaxe : MonoBehaviour
    {
        private GameManager _gameManager;
        private GameData GameData => _gameManager.GameData;

        public Action onSpeedChanged;
        public Action onWealthChanged;
        public Action onExperienceChanged;
        public Action onAutoClickChanged;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void AddDamage(int count)
        {
            var pickaxeState = _gameManager.PickaxeState;
            pickaxeState.DamageLevel += count;
            if (pickaxeState.MaxDamageLevel <= pickaxeState.DamageLevel) pickaxeState.MaxDamageLevel = pickaxeState.DamageLevel;

            #region PickaxeDamageLevel Event
            switch (pickaxeState.DamageLevel)
            {
                case 5:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_damage_5");
                    break;
                case 10:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_damage_10");
                    break;
                case 15:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_damage_15");
                    break;
                case 20:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_damage_20");
                    break;

            }
            #endregion

            onSpeedChanged?.Invoke();
        }

        public void AddWealth(int count)
        {
            var pickaxeState = _gameManager.PickaxeState;
            pickaxeState.WealthLevel += count;
            if (pickaxeState.MaxWealthLevel <= pickaxeState.WealthLevel) pickaxeState.MaxWealthLevel = pickaxeState.WealthLevel;

            #region PickaxeWealthLevel Event
            switch (pickaxeState.WealthLevel)
            {
                case 5:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_wealth_5");
                    break;
                case 10:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_wealth_10");
                    break;
                case 15:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_wealth_15");
                    break;
                case 20:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_wealth_20");
                    break;
            }
            #endregion

            onWealthChanged?.Invoke();
        }

        public void AddExperience(int count)
        {
            var pickaxeState = _gameManager.PickaxeState;
            pickaxeState.ExperienceLevel += count;
            if (pickaxeState.MaxExperienceLevel <= pickaxeState.ExperienceLevel) pickaxeState.MaxExperienceLevel = pickaxeState.ExperienceLevel;

            #region PickaxeExperienceLevel Event
            switch (pickaxeState.ExperienceLevel)
            {
                case 5:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_experience_5");
                    break;
                case 10:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_experience_10");
                    break;
                case 15:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_experience_15");
                    break;
                case 20:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_experience_20");
                    break;
            }
            #endregion

            onExperienceChanged?.Invoke();
        }

        public void AddAutoClick(int count)
        {
            var pickaxeState = _gameManager.PickaxeState;
            pickaxeState.AutoClickLevel += count;
            if (pickaxeState.MaxAutoClickLevel <= pickaxeState.AutoClickLevel) pickaxeState.MaxAutoClickLevel = pickaxeState.AutoClickLevel;

            #region PickaxeAutoClickLevel Event
            switch (pickaxeState.AutoClickLevel)
            {
                case 5:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_auto_click_5");
                    break;
                case 10:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_auto_click_10");
                    break;
                case 15:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Pickaxe_auto_click_15");
                    break;
            }
            #endregion

            onAutoClickChanged?.Invoke();
        }

        public int GetAutoClickPerMinute()
        {
            float value = (float)_gameManager.PickaxeState.AutoClickLevel / 15f;
            int clickInMinute = (int)(_gameManager.GameData.AutoClickPerMinuteCurve.Evaluate(value) * _gameManager.GameData.AutoClickUpgradeMaxClickPerMinute);
            return clickInMinute;
        }

        public float GetExperienceMultiplier()
        {
            float boosterMultiplier = 1f;
            var playerState = _gameManager.GameState.PlayerState;
            foreach (var activeBooster in playerState.BoosterStates)
            {
                var booster = _gameManager.Items.FirstOrDefault(e => e.Id == activeBooster.Id) as BoosterItemData;

                if (booster == null || booster.ExperienceMultiplier == 0)
                    continue;

                boosterMultiplier = booster.ExperienceMultiplier;
                break;
            }

            float experienceLevel = _gameManager.PickaxeState.ExperienceLevel;
            float multiplier = 1f + (experienceLevel - 1) * GameData.ExperienceMultiplierPerExpLevel; // с учЄтом улучшений кирки
            multiplier *= boosterMultiplier; // примен€ем бустер опыта

            var sharpeningRuneState = playerState.PickaxeState.Runes.FirstOrDefault(e => e.Id == "IF005"); // с учЄтом руны «ј“ќ„ ј
            if (sharpeningRuneState != null)
            {
                var activateRuneChance = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "activate_rune_chance");
                var activateRuneChanceState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "activate_rune_chance");

                var sharpeningRuneItem = _gameManager.Items.FirstOrDefault(e => e.Id == sharpeningRuneState.Id) as RuneItemData;

                if (Random.Range(0f, 100f) <= (sharpeningRuneItem.GetFirstValue(sharpeningRuneState.Level) + (activateRuneChance.Value * activateRuneChanceState.Level)))
                {
                    multiplier += sharpeningRuneItem.GetSecondValue(sharpeningRuneState.Level);
                }
            }

            return multiplier;
        }

        public float GetPickaxeMultiplier()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var pickaxeSpeedMultiplier = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "pickaxe_speed_multiplier");
            var pickaxeSpeedMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "pickaxe_speed_multiplier");

            float speedLevel = _gameManager.PickaxeState.DamageLevel;
            float multiplier = _gameManager.GameData.BasePickaxeSpeedMultiplier + ((speedLevel - 1) * GameData.PickaxeMultiplierSpeedPerLevel); // с учЄтом улучшений кирки
            multiplier *= 1f + ((pickaxeSpeedMultiplier.Value * pickaxeSpeedMultiplierState.Level) / 100f); // с учЄтом прокачек за перерождение

            return multiplier;
        }

        public bool IsCriticalDamage()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var criticalDamageChance = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "critical_damage_chance");
            var criticalDamageChanceState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "critical_damage_chance");

            float criticalChance = _gameManager.GameData.CriticalDamageChance + (criticalDamageChance.Value * criticalDamageChanceState.Level);

            return Random.Range(0f, 100f) <= criticalChance;
        }

        public bool KeyDropped()
        {
            return Random.Range(0f, 100f) < KeyDropChance();
        }

        public float KeyDropChance()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var keyDropChance = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "drop_key_chance");
            var keyDropChanceState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "drop_key_chance");

            float dropChance = GameData.AllKeyDropChances(); // шанс выбить ключ
            dropChance += keyDropChance.Value * keyDropChanceState.Level; // с учЄтом прокачек за перерождение

            var keyKeeperRuneState = playerState.PickaxeState.Runes.FirstOrDefault(e => e.Id == "IF002"); // с учЄтом руны  Ћё„Ќ» 
            if (keyKeeperRuneState != null)
            {
                var keyKeeperRuneItem = _gameManager.Items.FirstOrDefault(e => e.Id == keyKeeperRuneState.Id) as RuneItemData;
                dropChance += keyKeeperRuneItem.GetFirstValue(keyKeeperRuneState.Level);
            }

            return dropChance;
        }

        public RarityType GetDroppedKey()
        {
            float rand = Random.Range(0f, GameData.AllKeyDropChances());

            if (rand < GameData.BaseChanceToDropCommonKey)
            {
                return RarityType.Common;
            }

            rand -= GameData.BaseChanceToDropCommonKey;
            if (rand < GameData.BaseChanceToDropRareKey)
            {
                return RarityType.Rare;
            }

            rand -= GameData.BaseChanceToDropRareKey;
            if (rand < GameData.BaseChanceToDropEpicKey)
            {
                return RarityType.Epic;
            }

            rand -= GameData.BaseChanceToDropEpicKey;
            if (rand < GameData.BaseChanceToDropMythicalKey)
            {
                return RarityType.Mythical;
            }

            return RarityType.Legendary;
        }

        public bool MoneyDropped()
        {
            return Random.Range(0f, 100f) <= MoneyDropChance();
        }

        public float MoneyDropChance()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var mineralsDropChance = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "drop_minerals_chance");
            var mineralsDropChanceState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "drop_minerals_chance");

            int wealthLevel = _gameManager.PickaxeState.WealthLevel;
            float dropChance = GameData.BaseChanceToDropMoney + ((wealthLevel - 1) * GameData.MoneyChanceDropMultiplierPerWealthLevel); // с учЄтом улучшений кирки
            dropChance += mineralsDropChance.Value * mineralsDropChanceState.Level; // с учЄтом прокачек за перерождение

            var fortuneRuneState = playerState.PickaxeState.Runes.FirstOrDefault(e => e.Id == "IF001"); // с учЄтом руны ‘ќ–“”Ќј
            if (fortuneRuneState != null)
            {
                var fortuneRuneItem = _gameManager.Items.FirstOrDefault(e => e.Id == fortuneRuneState.Id) as RuneItemData;
                dropChance += fortuneRuneItem.GetFirstValue(fortuneRuneState.Level);
            }

            if (!playerState.CompletedTutorialsIds.Contains("upgrades_tutorial"))
                dropChance *= 3f;
            else if (!playerState.CompletedTutorialsIds.Contains("cave_upgrade_tutorial"))
                dropChance *= 2f;

            return dropChance;
        }

        public int GetDroppedMoneyCount()
        {
            float boosterMultiplier = 1f;
            var playerState = _gameManager.GameState.PlayerState;
            foreach(var activeBooster in playerState.BoosterStates)
            {
                var booster = _gameManager.Items.FirstOrDefault(e => e.Id == activeBooster.Id) as BoosterItemData;

                if (booster == null || booster.MoneyMultiplier == 0)
                    continue;

                boosterMultiplier = booster.MoneyMultiplier;
                break;
            }

            int wealthLevel = _gameManager.PickaxeState.WealthLevel;
            int finalCount = GameData.BaseDroppedMoneyCount;
            int maxMoneyStepCount = Mathf.Clamp(Mathf.FloorToInt((float)(wealthLevel - 1) / 1.4f), 1, int.MaxValue);

            float chanceToMoreMoney = GameData.BaseChanceToMoreMoneyCountDropped + ((wealthLevel - 1) * GameData.MoreMoneyCountDroppedChanceMultiplierPerWealthLevel);
            for (int i = 0; i < maxMoneyStepCount; i++)
            {
                if (Random.Range(0f, 100) > chanceToMoreMoney)
                    break;

                finalCount *= 2;
            }

            finalCount = (int)(finalCount * boosterMultiplier); // примен€ем бустер денег

            return finalCount;
        }
    }
}