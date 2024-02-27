using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

namespace CaveMiner
{
    [System.Serializable]
    public class GameState
    {
        public bool VibrationIsOn = true;
        public bool SoundsIsOn = true;
        public bool QuickOpeningIsOn;
        public bool NoAds;
        public bool SpeedRecycle;
        public bool IsRated;
        public string ViewedVersionUpdate;
        public GraphicsQuality GraphicsQuality = GraphicsQuality.High;
        public PlayerState PlayerState;
    }

    [System.Serializable]
    public class PlayerState
    {
        public ObscuredInt LastDailyRewardTimestamp;
        public ObscuredInt DailyRewardStrike;
        public ObscuredInt LastSaveTimestamp;
        public ObscuredFloat PlaytimeSeconds;
        public ObscuredInt Money;
        public ObscuredInt Experience;
        public ObscuredInt CaveLevel;
        public ObscuredInt RebirthCount;
        public ObscuredString CurrentCave;
        public ObscuredInt RebirthPoints;
        public ObscuredInt CommonKeyCount;
        public ObscuredInt RareKeyCount;
        public ObscuredInt EpicKeyCount;
        public ObscuredInt MythicalKeyCount;
        public ObscuredInt LegendaryKeyCount;
        public PlayerStatsState Stats = new PlayerStatsState();
        public RecyclingState RecyclingState = new RecyclingState();
        public StorageState StorageState = new StorageState();
        public PickaxeState PickaxeState;
        public GameShopState GameShop; // Игровой магазин
        public List<BoosterState> BoosterStates = new List<BoosterState>();
        public List<RebirthUpgradeState> RebirthUpgradeStates = new List<RebirthUpgradeState>();
        public List<CaveState> CaveStates = new List<CaveState>(); // Какие шахты были разблокированны за всё время
        public List<ItemState> Items = new List<ItemState>(); // Предметы игрока
        public List<ResourceItemState> ResourceItems = new List<ResourceItemState>(); // ресурсы
        public List<string> CraftedItemsIds = new List<string>(); // айдишники скрафченых предметов
        public List<TaskState> Tasks = new List<TaskState>(); // задания
        public List<string> CompletedTutorialsIds = new List<string>();
    }

    [System.Serializable]
    public class StorageState
    {
        public int Level = 1;
        public ObscuredFloat ExperiencePerSecond;
    }

    [System.Serializable]
    public class RecyclingState
    {
        public ObscuredInt LastTimestamp;
        public int RecycleType;
        public ResourceItemState ResourceItem = new ResourceItemState();
        public ObscuredFloat Minerals;
        public ObscuredFloat Experience;
        public float FirstPlaceResourceRecycleTime;
        public int RewardAdsLeft;
        public int FirstWatchRewardAdTimestamp;
    }

    [System.Serializable]
    public class PlayerStatsState
    {
        public ObscuredInt BlockDestroyed;
        public ObscuredInt ClickCount;
        public ObscuredInt RecycleItemsCount;
        public ObscuredInt PlacedItemsToStorage;
        public ObscuredInt RebirthCount;
        public ObscuredInt CombineRuneCount;
        public ObscuredInt CompletedTasks;
        public ObscuredInt UseDynamiteCount;
        public ObscuredInt ChestOpenedCount;
        public ObscuredInt ExchangedKeyCount;
        public ObscuredInt BuyItemsInGameShop;
        public ObscuredInt GameShopUpdateItemsCount;
        public ObscuredInt BreakRuneCount;
        public ObscuredInt SellItemsCount;
        public ObscuredInt UseBoostersCount;
        public ObscuredInt DeleteItemsCount;
    }

    [System.Serializable]
    public class CaveState
    {
        public ObscuredString Id;
        public ObscuredInt Level;
        public float DifficultyMultiplier = 1f;
        public int PrizeCount;
        public int MaxDepth;
        public int CurrentDepth;
    }

    [System.Serializable]
    public class TaskState
    {
        public ObscuredString Id;
        public ObscuredString Category;
        public ObscuredString NeedValue;
        public ObscuredBool CompleteNotificated;
        public ObscuredInt CurrentValue;
        public ObscuredInt TargetValue;
        public ObscuredInt RewardValue;
    }

    [System.Serializable]
    public class OfflineIncomeState
    {
        public int Minerals;
        public int Experience;
        public List<ResourceItemState> Resources = new List<ResourceItemState>();
        public int CommonKey;
        public int RareKey;
        public int EpicKey;
        public int MythicalKey;
        public int LegendaryKey;

        public int KeyCount()
        {
            return CommonKey + RareKey + EpicKey + MythicalKey + LegendaryKey;
        }
    }

    [System.Serializable]
    public class BoosterState
    {
        public ObscuredString Id;
        public ObscuredInt ActiveTimestamp;
    }

    [System.Serializable]
    public class GameShopState
    {
        public ObscuredInt LastUpdateTimestamp;
        public List<ShopItemState> Items = new List<ShopItemState>();
    }

    [System.Serializable]
    public class PickaxeState
    {
        public ObscuredInt DamageLevel;
        public ObscuredInt WealthLevel;
        public ObscuredInt ExperienceLevel;
        public ObscuredInt AutoClickLevel;
        public ObscuredInt MaxDamageLevel; // максимальный уровень который был
        public ObscuredInt MaxWealthLevel; // максимальный уровень который был
        public ObscuredInt MaxExperienceLevel; // максимальный уровень который был
        public ObscuredInt MaxAutoClickLevel; // максимальный уровень который был
        public PickaxeRuneState[] Runes;
    }

    [System.Serializable]
    public class PickaxeRuneState
    {
        public ObscuredString Id;
        public ObscuredInt Level;
    }

    [System.Serializable]
    public class RebirthUpgradeState
    {
        public ObscuredString Id;
        public ObscuredInt Level;
    }

    [System.Serializable]
    public class ItemState
    {
        public ObscuredString Id;
        public ObscuredInt Count;
        public ObscuredString CustomValue;
    }

    [System.Serializable]
    public class ResourceItemState
    {
        public ObscuredString Id;
        public ObscuredInt Count;
    }

    [System.Serializable]
    public class ShopItemState
    {
        public ObscuredString Id;
        public ObscuredInt Count;
        public ObscuredInt Price;
        public ObscuredString CustomValue;
    }
}