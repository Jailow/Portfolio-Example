using System.Linq;
using UnityEngine;

namespace CaveMiner
{
    public class AchievementManager : Singleton<AchievementManager>
    {
        public void CheckAllConditions(GameManager gameManager)
        {
            var playerState = gameManager.GameState.PlayerState;

            CheckDailyRewardStrikeCondition(playerState.DailyRewardStrike);

            CheckBlockDestroyedCondition(playerState.Stats.BlockDestroyed);
            CheckClickCountCondition(playerState.Stats.ClickCount);
            CheckRecycleItemsCountCondition(playerState.Stats.RecycleItemsCount);
            CheckPlacedItemsToStorageCountCondition(playerState.Stats.PlacedItemsToStorage);
            CheckRebirthCountCondition(playerState.Stats.RebirthCount);
            CheckCombineRuneCountCondition(playerState.Stats.CombineRuneCount);
            CheckCompleteTasksCountCondition(playerState.Stats.CompletedTasks);
            CheckUseDynamiteCountCondition(playerState.Stats.UseDynamiteCount);
            CheckOpenedChestsCountCondition(playerState.Stats.ChestOpenedCount);
            CheckExchangeKeyCountCondition(playerState.Stats.ExchangedKeyCount);
            CheckBuyItemsInGameShopCountCondition(playerState.Stats.BuyItemsInGameShop);
            CheckUpdateGameShopCountCondition(playerState.Stats.GameShopUpdateItemsCount);
            CheckBreakRuneCountCondition(playerState.Stats.BreakRuneCount);
            CheckSellItemsCount(playerState.Stats.SellItemsCount);
            CheckUseBoostersCountCondition(playerState.Stats.UseBoostersCount);
            CheckDeleteItemsCount(playerState.Stats.DeleteItemsCount);

            CheckMaxRuneCountCondition(playerState.PickaxeState);
            CheckMaxRuneLevelCondition(gameManager, playerState.PickaxeState);

            CheckCaveLevelCondition(playerState);
            CheckCaveUnlockedCondition(playerState);

            CheckPickaxeDamageLevelCondition(playerState.PickaxeState.DamageLevel);
            CheckPickaxeWealthLevelCondition(playerState.PickaxeState.WealthLevel);
            CheckPickaxeExperienceLevelCondition(playerState.PickaxeState.ExperienceLevel);
            CheckPickaxeAutoClickLevelCondition(playerState.PickaxeState.AutoClickLevel);
        }

        public void CheckDailyRewardStrikeCondition(int dailyRewardStrike)
        {
            if(dailyRewardStrike >= 7) // страйк взятия ежедневной награды 7
            {
                CompleteAchievement(GPGSIds.achievement_frequent_visitor);
            }
        }

        public void CheckCaveUnlockedCondition(PlayerState playerState)
        {
            foreach (var caveState in playerState.CaveStates)
            {
                if(caveState.Id == "C005") // Достигнут 5 уровень шахты
                {
                    CompleteAchievement(GPGSIds.achievement_mine_scout);
                }

                if (caveState.Id == "C010") // Достигнут 10 уровень шахты
                {
                    CompleteAchievement(GPGSIds.achievement_master_of_depths);
                }

                if (caveState.Id == "C015") // Достигнут 15 уровень шахты
                {
                    CompleteAchievement(GPGSIds.achievement_miner_conqueror);
                }
            }
        }

        public void CheckPickaxeAutoClickLevelCondition(int autoClickLevel)
        {
            if(autoClickLevel >= 5) // Авто клик кирки на 5 уровень
            {
                CompleteAchievement(GPGSIds.achievement_autoclick_5);
            }

            if (autoClickLevel >= 10) // Авто клик кирки на 10 уровень
            {
                CompleteAchievement(GPGSIds.achievement_autoclick_10);
            }

            if (autoClickLevel >= 15) // Авто клик кирки на 15 уровень
            {
                CompleteAchievement(GPGSIds.achievement_autoclick_15);
            }
        }

        public void CheckPickaxeExperienceLevelCondition(int experienceLevel)
        {
            if(experienceLevel >= 15) // опыт кирки на 15 уровень
            {
                CompleteAchievement(GPGSIds.achievement_experience_15);
            }

            if (experienceLevel >= 30) // опыт кирки на 30 уровень
            {
                CompleteAchievement(GPGSIds.achievement_experience_30);
            }
        }

        public void CheckPickaxeDamageLevelCondition(int damageLevel)
        {
            if(damageLevel >= 10) // урон кирки на 10 уровень
            {
                CompleteAchievement(GPGSIds.achievement_damage_10);
            }

            if (damageLevel >= 20) // урон кирки на 20 уровень
            {
                CompleteAchievement(GPGSIds.achievement_damage_20);
            }
        }

        public void CheckPickaxeWealthLevelCondition(int wealthLevel)
        {
            if (wealthLevel >= 10) // богатство кирки на 10 уровень
            {
                CompleteAchievement(GPGSIds.achievement_wealth_10);
            }

            if (wealthLevel >= 20) // богатство кирки на 20 уровень
            {
                CompleteAchievement(GPGSIds.achievement_wealth_20);
            }
        }

        public void CheckCaveLevelCondition(PlayerState playerState)
        {
            foreach(var caveState in playerState.CaveStates)
            {
                if(caveState.Level >= 5) // Прокачана шахта на 5 уровень
                {
                    CompleteAchievement(GPGSIds.achievement_upgraded_mine);
                }

                if(caveState.Level >= 10)// Прокачана шахта на 10 уровень
                {
                    CompleteAchievement(GPGSIds.achievement_maximum_mine);
                }
            }
        }

        public void CheckMaxRuneLevelCondition(GameManager gameManager, PickaxeState pickaxeState)
        {
            for(int i = 0; i < pickaxeState.Runes.Length; i++)
            {
                if (string.IsNullOrEmpty(pickaxeState.Runes[i].Id))
                    continue;

                if(pickaxeState.Runes[i].Level >= (gameManager.Items.FirstOrDefault(e => e.Id == pickaxeState.Runes[i].Id) as RuneItemData).MaxLevel) // Максимальный уровень руны
                {
                    CompleteAchievement(GPGSIds.achievement_powerful_effect);
                    break;
                }
            }
        }

        public void CheckMaxRuneCountCondition(PickaxeState pickaxeState)
        {
            if (pickaxeState.Runes.All(e => !string.IsNullOrEmpty(e.Id) && e.Level > 0)) // Максимум рун в слотах
            {
                CompleteAchievement(GPGSIds.achievement_maximum_runes);
            }
        }

        public void CheckDeleteItemsCount(int itemsCount)
        {
            if(itemsCount >= 1000) // выкинуть 1000 предметов
            {
                CompleteAchievement(GPGSIds.achievement_heap_of_garbage);
            }
        }

        public void CheckUseBoostersCountCondition(int boosterCount)
        {
            if(boosterCount >= 10) // использовать 10 бустеров
            {
                CompleteAchievement(GPGSIds.achievement_need_more);
            }

            if (boosterCount >= 100) // использовать 100 бустеров
            {
                CompleteAchievement(GPGSIds.achievement_constant_multiplication);
            }

            if (boosterCount >= 1000) // использовать 1к бустеров
            {
                CompleteAchievement(GPGSIds.achievement_booster_expert);
            }
        }

        public void CheckPlacedItemsToStorageCountCondition(int count)
        {
            if (count >= 1000) // Положить 1к ресурсов в хранилище
            {
                CompleteAchievement(GPGSIds.achievement_modest_reserve);
            }

            if (count >= 5000) // Положить 5к ресурсов в хранилище
            {
                CompleteAchievement(GPGSIds.achievement_growing_reserves);
            }

            if (count >= 25000) // Положить 25к ресурсов в хранилище
            {
                CompleteAchievement(GPGSIds.achievement_storage_master);
            }

            if (count >= 100000) // Положить 100к ресурсов в хранилище
            {
                CompleteAchievement(GPGSIds.achievement_grand_steward);
            }
        }

        public void CheckSellItemsCount(int sellItemsCount)
        {
            if(sellItemsCount >= 25) // Продать 25 вещей
            {
                CompleteAchievement(GPGSIds.achievement_novice_seller);
            }

            if (sellItemsCount >= 100) // Продать 100 вещей
            {
                CompleteAchievement(GPGSIds.achievement_pro_seller);
            }

            if (sellItemsCount >= 1000) // Продать 1к вещей
            {
                CompleteAchievement(GPGSIds.achievement_trading_tycoon);
            }

            if (sellItemsCount >= 10000) // Продать 10к вещей
            {
                CompleteAchievement(GPGSIds.achievement_trading_emperor);
            }
        }

        public void CheckBreakRuneCountCondition(int breakRuneCount)
        {
            if(breakRuneCount >= 5) // Сломать 5 рун
            {
                CompleteAchievement(GPGSIds.achievement_unlucky);
            }

            if (breakRuneCount >= 10) // Сломать 10 рун
            {
                CompleteAchievement(GPGSIds.achievement_loser);
            }

            if (breakRuneCount >= 25) // Сломать 25 рун
            {
                CompleteAchievement(GPGSIds.achievement_rune_breaker);
            }

            if (breakRuneCount >= 50) // Сломать 50 рун
            {
                CompleteAchievement(GPGSIds.achievement_pile_of_broken_runes);
            }
        }

        public void CheckUpdateGameShopCountCondition(int updateCount)
        {
            if(updateCount >= 1) // обновить ассортимент магазина 1 раз
            {
                CompleteAchievement(GPGSIds.achievement_want_something_else);
            }

            if (updateCount >= 5) // обновить ассортимент магазина 5 раз
            {
                CompleteAchievement(GPGSIds.achievement_more_variety);
            }

            if (updateCount >= 10) // обновить ассортимент магазина 10 раз
            {
                CompleteAchievement(GPGSIds.achievement_different_assortment);
            }

            if (updateCount >= 25) // обновить ассортимент магазина 25 раз
            {
                CompleteAchievement(GPGSIds.achievement_this_doesnt_suit_me);
            }

            if (updateCount >= 50) // обновить ассортимент магазина 50 раз
            {
                CompleteAchievement(GPGSIds.achievement_constant_update);
            }
        }

        public void CheckBuyItemsInGameShopCountCondition(int buyItemsCount)
        {
            if(buyItemsCount >= 5) // купить вещи в магазине 5 раз
            {
                CompleteAchievement(GPGSIds.achievement_store_explore);
            }

            if (buyItemsCount >= 10) // купить вещи в магазине 10 раз
            {
                CompleteAchievement(GPGSIds.achievement_buyer);
            }

            if (buyItemsCount >= 25) // купить вещи в магазине 25 раз
            {
                CompleteAchievement(GPGSIds.achievement_loyal_customer);
            }

            if (buyItemsCount >= 50) // купить вещи в магазине 50 раз
            {
                CompleteAchievement(GPGSIds.achievement_shopaholic);
            }

            if (buyItemsCount >= 100) // купить вещи в магазине 100 раз
            {
                CompleteAchievement(GPGSIds.achievement_legendary_shopaholic);
            }
        }

        public void CheckExchangeKeyCountCondition(int exchangeKeyCount)
        {
            if(exchangeKeyCount >= 5) // обменять ключи 5 раз
            {
                CompleteAchievement(GPGSIds.achievement_exchanger);
            }

            if (exchangeKeyCount >= 25) // обменять ключи 25 раз
            {
                CompleteAchievement(GPGSIds.achievement_experienced_trader);
            }

            if (exchangeKeyCount >= 100) // обменять ключи 100 раз
            {
                CompleteAchievement(GPGSIds.achievement_trader);
            }
        }

        public void CheckOpenedChestsCountCondition(int openedChestsCount)
        {
            if(openedChestsCount >= 10) // Открыть 10 сундуков
            {
                CompleteAchievement(GPGSIds.achievement_chest_opener);
            }

            if (openedChestsCount >= 50) // Открыть 50 сундуков
            {
                CompleteAchievement(GPGSIds.achievement_master_of_openings);
            }

            if (openedChestsCount >= 100) // Открыть 100 сундуков
            {
                CompleteAchievement(GPGSIds.achievement_chest_expert);
            }

            if (openedChestsCount >= 500) // Открыть 500 сундуков
            {
                CompleteAchievement(GPGSIds.achievement_treasure_hunter);
            }

            if (openedChestsCount >= 1000) // Открыть 1к сундуков
            {
                CompleteAchievement(GPGSIds.achievement_lord_of_chests);
            }
        }

        public void CheckUseDynamiteCountCondition(int useDynamiteCount)
        {
            if(useDynamiteCount >= 500) // использовать 500 динамита
            {
                CompleteAchievement(GPGSIds.achievement_boom);
            }

            if (useDynamiteCount >= 1000) // использовать 1к динамита
            {
                CompleteAchievement(GPGSIds.achievement_detonator);
            }

            if (useDynamiteCount >= 5000) // использовать 5к динамита
            {
                CompleteAchievement(GPGSIds.achievement_master_of_explosions);
            }

            if (useDynamiteCount >= 10000) // использовать 10к динамита
            {
                CompleteAchievement(GPGSIds.achievement_experienced_pyrotechnician);
            }

            if (useDynamiteCount >= 50000) // использовать 50к динамита
            {
                CompleteAchievement(GPGSIds.achievement_explosive_catastrophe);
            }

            if (useDynamiteCount >= 100000) // использовать 100к динамита
            {
                CompleteAchievement(GPGSIds.achievement_nuclear_explosion);
            }
        }

        public void CheckCompleteTasksCountCondition(int taskCompleted)
        {
            if(taskCompleted >= 10) // Выполнить 10 заданий
            {
                CompleteAchievement(GPGSIds.achievement_novice_executor);
            }

            if (taskCompleted >= 50) // Выполнить 50 заданий
            {
                CompleteAchievement(GPGSIds.achievement_hard_worker);
            }

            if (taskCompleted >= 250) // Выполнить 250 заданий
            {
                CompleteAchievement(GPGSIds.achievement_task_expert);
            }

            if (taskCompleted >= 1000) // Выполнить 1к заданий
            {
                CompleteAchievement(GPGSIds.achievement_task_king);
            }
        }

        public void CheckCombineRuneCountCondition(int combineRuneCount)
        {
            if(combineRuneCount >= 5) // обьединить руны 5 раз
            {
                CompleteAchievement(GPGSIds.achievement_better_rune);
            }

            if (combineRuneCount >= 10) // обьединить руны 10 раз
            {
                CompleteAchievement(GPGSIds.achievement_combiner);
            }

            if (combineRuneCount >= 25) // обьединить руны 25 раз
            {
                CompleteAchievement(GPGSIds.achievement_rune_combination);
            }

            if (combineRuneCount >= 50) // обьединить руны 50 раз
            {
                CompleteAchievement(GPGSIds.achievement_rune_combo);
            }

            if (combineRuneCount >= 100) // обьединить руны 100 раз
            {
                CompleteAchievement(GPGSIds.achievement_love_for_combinations);
            }
        }

        public void CheckRebirthCountCondition(int rebirthCount)
        {
            if (rebirthCount >= 1) // переродиться 1 раз
            {
                CompleteAchievement(GPGSIds.achievement_from_the_beginning);
            }

            if (rebirthCount >= 5) // переродиться 5 раз
            {
                CompleteAchievement(GPGSIds.achievement_for_the_fifth_time);
            }

            if (rebirthCount >= 10) // переродиться 10 раз
            {
                CompleteAchievement(GPGSIds.achievement_ten_lives);
            }

            if (rebirthCount >= 25) // переродиться 25 раз
            {
                CompleteAchievement(GPGSIds.achievement_rebirth_master);
            }

            if (rebirthCount >= 50) // переродиться 50 раз
            {
                CompleteAchievement(GPGSIds.achievement_immortal);
            }

            if (rebirthCount >= 100) // переродиться 100 раз
            {
                CompleteAchievement(GPGSIds.achievement_spirit_of_rebirth);
            }
        }

        public void CheckRecycleItemsCountCondition(int itemsCount)
        {
            if(itemsCount >= 1000) // переработать 1к ресурсов
            {
                CompleteAchievement(GPGSIds.achievement_initial_processing);
            }

            if (itemsCount >= 5000) // переработать 5к ресурсов
            {
                CompleteAchievement(GPGSIds.achievement_utilizer);
            }

            if (itemsCount >= 25000) // переработать 25к ресурсов
            {
                CompleteAchievement(GPGSIds.achievement_ecologist);
            }

            if (itemsCount >= 100000) // переработать 100к ресурсов
            {
                CompleteAchievement(GPGSIds.achievement_recycling_master);
            }
        }

        public void CheckBlockDestroyedCondition(int blockDestroyed)
        {
            if(blockDestroyed >= 10000) // Сломать 10к блоков
            {
                CompleteAchievement(GPGSIds.achievement_into_fragments);
            }

            if (blockDestroyed >= 25000) // Сломать 25к блоков
            {
                CompleteAchievement(GPGSIds.achievement_lonely_ruins);
            }

            if (blockDestroyed >= 50000) // Сломать 50к блоков
            {
                CompleteAchievement(GPGSIds.achievement_demolition_master);
            }

            if (blockDestroyed >= 100000) // Сломать 100к блоков
            {
                CompleteAchievement(GPGSIds.achievement_mega_destructor);
            }

            if (blockDestroyed >= 250000) // Сломать 250к блоков
            {
                CompleteAchievement(GPGSIds.achievement_whirlwind_of_destruction);
            }

            if (blockDestroyed >= 500000) // Сломать 500к блоков
            {
                CompleteAchievement(GPGSIds.achievement_block_apocalypse);
            }

            if (blockDestroyed >= 1000000) // Сломать 1м блоков
            {
                CompleteAchievement(GPGSIds.achievement_reality_breaker);
            }
        }

        public void CheckClickCountCondition(int clickCount)
        {
            if(clickCount >= 1000) // кликнуть 1к раз
            {
                CompleteAchievement(GPGSIds.achievement_novice);
            }

            if (clickCount >= 5000) // кликнуть 5к раз
            {
                CompleteAchievement(GPGSIds.achievement_experienced_clicker);
            }

            if (clickCount >= 25000) // кликнуть 25к раз
            {
                CompleteAchievement(GPGSIds.achievement_professional_clicker);
            }

            if (clickCount >= 50000) // кликнуть 50к раз
            {
                CompleteAchievement(GPGSIds.achievement_clicker_storm);
            }

            if (clickCount >= 100000) // кликнуть 100к раз
            {
                CompleteAchievement(GPGSIds.achievement_clicker_madness);
            }

            if (clickCount >= 250000) // кликнуть 250к раз
            {
                CompleteAchievement(GPGSIds.achievement_click_lord);
            }

            if (clickCount >= 500000) // кликнуть 500к раз
            {
                CompleteAchievement(GPGSIds.achievement_infinite_clicker);
            }

            if (clickCount >= 1000000) // кликнуть 1м раз
            {
                CompleteAchievement(GPGSIds.achievement_million_clicks);
            }
        }

        public void CompleteAchievement(string id)
        {
            Social.ReportProgress(id, 100, null);
        }
    }
}