using UnityEngine;
using CaveMiner.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using CaveMiner.Secure;
using CodeStage.AntiCheat.Storage;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Purchasing;
using System.Collections;
using NatSuite.Examples;

namespace CaveMiner
{
    public class GameManager : MonoBehaviour
    {
        private ReplayCam _replayCam;
        private UIManager _uiManager;
        private AntiCheatManager _antiCheatManager;
        public CaveController CaveController { get; private set; }
        public TaskData[] Tasks { get; private set; }
        public BlockData[] BlockDatas { get; private set; }
        public ResourceItemData[] ResourceItems { get; private set; }
        public ItemData[] Items { get; private set; }
        public RebirthUpgrade[] RebirthUpgrades { get; private set; }
        public GameData GameData { get; private set; }
        public GameState GameState { get; private set; }
        public SoundData SoundData { get; private set; }
        public Pickaxe Pickaxe { get; private set; }
        public CaveData[] Caves { get; private set; }
        public PickaxeState PickaxeState => GameState.PlayerState.PickaxeState;

        public Action<int> onExperienceChanged;
        public Action<int> onMoneyChanged;
        public Action<int> onRebirthPointsChanged;
        public Action<string> onAddedRebirthUpgradeLevel;
        public Action<ItemState> onItemAdded;
        public Action<ResourceItemState> onResourceItemAdded;
        public Action<string> onCaveLevelChanged;
        public Action onCavePrizeChanged;
        public Action<GraphicsQuality> onGraphicsQualityChanged;
        public Action onCaveDifficultyMultiplierChanged;
        public Action onStorageLevelChanged;
        public Action onCaveChanged;
        public Action onKeyChanged;
        public Action onActiveBoostersChanged;
        public Action onExperiencePerSecondChanged;

        private float _time;
        private int _currentSaveCount;
        private const int _saveCountToCloudSave = 3;
        private const int _timeToAutoSave = 20;

        public float ExperiencePerSecondMultiplier { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 144;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Physics.autoSyncTransforms = true;
            Physics.reuseCollisionCallbacks = true;

#if !UNITY_EDITOR
            Vibration.Init();
#endif
            TutorialController.Instance.onTutorialCompleted += OnTutorialCompleted;
            IAPController.Instance.onPurchaseCompleted += OnPurchaseCompleted;

            GameData = Resources.Load<GameData>("GameData");
            SoundData = Resources.Load<SoundData>("SoundData");
            RebirthUpgrades = Resources.LoadAll<RebirthUpgrade>("Data/RebirthUpgrades");
            //CraftItems = Resources.LoadAll<CraftItemData>("Data/CraftItems").OrderBy(item => int.Parse(item.name.Substring("craft_item_".Length))).ToArray();
            Caves = Resources.LoadAll<CaveData>("Data/Caves");
            Tasks = Resources.LoadAll<TaskData>("Data/Tasks");
            BlockDatas = Resources.LoadAll<BlockData>("Data/Blocks");
            var allItems = Resources.LoadAll<ItemData>("Data/Items");
            Items = allItems.Where(e => e.ItemType != ItemType.Resource).ToArray();
            var resourceItems = allItems.Where(e => e.ItemType == ItemType.Resource).ToArray();
            ResourceItems = new ResourceItemData[resourceItems.Length];
            for(int i = 0; i < resourceItems.Length; i++)
            {
                ResourceItems[i] = resourceItems[i] as ResourceItemData;
            }

            _antiCheatManager = FindObjectOfType<AntiCheatManager>();
            Pickaxe = FindObjectOfType<Pickaxe>();
            _uiManager = FindObjectOfType<UIManager>();
            CaveController = FindObjectOfType<CaveController>();

            CaveController.onBlockDestroyed += OnBlockDestroyed;
            CaveController.onKeyDropped += OnKeyDropped;

            Pickaxe.Init(this);

            LoadOrCreateNewGame();

            CaveController.Init(this, _uiManager);
            _uiManager.Init(this, CaveController);

            CaveController.SpawnRandomBlock();

            _uiManager.ShowGame();

            _replayCam = FindObjectOfType<ReplayCam>();
            _replayCam.Init(Screen.width, Screen.height, _uiManager.Canvas.worldCamera);
            _replayCam.StartRecording();

            Invoke(nameof(StopRecording), 30f);
        }

        private void StopRecording()
        {
            _replayCam.StopRecording();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _replayCam.StopRecording();
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddExperience(10000000);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                AddItem("I002", 50, string.Empty);
            }
#endif

            GameState.PlayerState.PlaytimeSeconds += Time.deltaTime;
            _time += Time.deltaTime;

            if (_time >= _timeToAutoSave && GameState != null)
            {
                _time = 0f;
                SaveGame();
            }
        }

        private IEnumerator ExperiencePerSecond()
        {
            var second = new WaitForSecondsRealtime(1f);

            var storageState = GameState.PlayerState.StorageState;

            while (true)
            {
                yield return second;

                if (storageState.ExperiencePerSecond > 0)
                    AddExperience((int)(storageState.ExperiencePerSecond));
            }
        }

        private IEnumerator CheckAchievements()
        {
            var second = new WaitForSecondsRealtime(5f);
            while (true)
            {
                yield return second;

                var playerState = GameState.PlayerState;

                AchievementManager.Instance.CheckBlockDestroyedCondition(playerState.Stats.BlockDestroyed);
                AchievementManager.Instance.CheckClickCountCondition(playerState.Stats.ClickCount);
                AchievementManager.Instance.CheckUseDynamiteCountCondition(playerState.Stats.UseDynamiteCount);
            }
        }

        private void OnBlockDestroyed(BlockData blockData)
        {
            var playerState = GameState.PlayerState;

            foreach (var task in playerState.Tasks)
            {
                var taskData = Tasks.FirstOrDefault(e => e.Id == task.Id);

                if (taskData.TaskType != TaskType.DestroyBlocks || task.NeedValue != blockData.Id)
                    continue;

                task.CurrentValue++;

                if (task.CurrentValue >= task.TargetValue && !task.CompleteNotificated) // Задание выполнено
                {
                    task.CompleteNotificated = true;
                    _uiManager.TaskCompleted(_uiManager.GetTaskName(taskData, task));
                }
            }
        }

        public void OnHitBlock()
        {
            var playerState = GameState.PlayerState;

            foreach (var task in playerState.Tasks)
            {
                var taskData = Tasks.FirstOrDefault(e => e.Id == task.Id);

                if (taskData.TaskType != TaskType.ClickCount)
                    continue;

                task.CurrentValue++;

                if (task.CurrentValue >= task.TargetValue && !task.CompleteNotificated) // Задание выполнено
                {
                    task.CompleteNotificated = true;
                    _uiManager.TaskCompleted(_uiManager.GetTaskName(taskData, task));
                }
            }
        }

        public void OnChestOpened(RarityType rarityType)
        {
            var playerState = GameState.PlayerState;

            foreach (var task in playerState.Tasks)
            {
                var taskData = Tasks.FirstOrDefault(e => e.Id == task.Id);

                if (taskData.TaskType != TaskType.OpenChests || (RarityType)Enum.Parse(typeof(RarityType), task.NeedValue) != rarityType)
                    continue;

                task.CurrentValue++;

                if (task.CurrentValue >= task.TargetValue && !task.CompleteNotificated) // Задание выполнено
                {
                    task.CompleteNotificated = true;
                    _uiManager.TaskCompleted(_uiManager.GetTaskName(taskData, task));
                }
            }
        }

        private void OnKeyDropped(RarityType rarityType)
        {
            var playerState = GameState.PlayerState;

            foreach (var task in playerState.Tasks)
            {
                var taskData = Tasks.FirstOrDefault(e => e.Id == task.Id);

                if (taskData.TaskType != TaskType.DropKeys || (RarityType)Enum.Parse(typeof(RarityType), task.NeedValue) != rarityType)
                    continue;

                task.CurrentValue++;

                if(task.CurrentValue >= task.TargetValue && !task.CompleteNotificated) // Задание выполнено
                {
                    task.CompleteNotificated = true;
                    _uiManager.TaskCompleted(_uiManager.GetTaskName(taskData, task));
                }
            }
        }

        private void OnPurchaseCompleted(Product product)
        {
            AmplitudeManager.Instance.EventRevenue(product.definition.id, 1, (double)product.metadata.localizedPrice);

            switch (product.definition.id)
            {
                case "5000_minerals":
                    AddMoney(5000);
                    break;
                case "10000_minerals":
                    AddMoney(10000);
                    break;
                case "50000_minerals":
                    AddMoney(50000);
                    break;
                case "2_epic_keys":
                    AddKey(RarityType.Epic, 2);
                    break;
                case "2_mythical_keys":
                    AddKey(RarityType.Mythical, 2);
                    break;
                case "2_legendary_keys":
                    AddKey(RarityType.Legendary, 2);
                    break;
                case "no_ads":
                    GameState.NoAds = true;
                    CleverAdsSolutionsManager.Instance.NoAds = true;
                    SaveGame();
                    break;
                case "recycle_x5":
                    GameState.SpeedRecycle = true;
                    SaveGame();
                    break;
            }
        }

        public void SetGraphicsQuality(GraphicsQuality quality)
        {
            GameState.GraphicsQuality = quality;

            onGraphicsQualityChanged?.Invoke(quality);
        }

        private void LoadOrCreateNewGame()
        {
            bool newGame = false;

            GameState cloudGameState = null;

#if !UNITY_EDITOR
            if (GooglePlayGamesManager.Instance.Data != null)
            {
                try
                {
                    string cloudSave = System.Text.Encoding.UTF8.GetString(GooglePlayGamesManager.Instance.Data);
                    string cloudJson = ObscuredString.Decrypt(cloudSave.ToCharArray(), AntiCheatManager.Hash);
                    cloudGameState = JsonUtility.FromJson<GameState>(cloudJson);
                }
                catch
                {
                    Debug.LogWarning("Can't load cloud save data (GooglePlay)");
                }
            }

            if (FirebaseManager.Instance.IsAuthenticated && FirebaseManager.Instance.Data != null)
            {
                try
                {
                    string cloudSave = System.Text.Encoding.UTF8.GetString(FirebaseManager.Instance.Data);
                    string cloudJson = ObscuredString.Decrypt(cloudSave.ToCharArray(), AntiCheatManager.Hash);
                    var firebaseGameState = JsonUtility.FromJson<GameState>(cloudJson);

                    if (cloudGameState == null || firebaseGameState.PlayerState.PlaytimeSeconds > cloudGameState.PlayerState.PlaytimeSeconds)
                        cloudGameState = firebaseGameState;
                }
                catch
                {
                    Debug.LogWarning("Can't load cloud save data (Firebase)");
                }
            }
#endif

            if (ObscuredPrefs.HasKey("game_state") || cloudGameState != null)
            {
                GameState localGameState = null;
                // Загружаем текущую игру
                string gameStateJson = ObscuredPrefs.Get<string>("game_state");
                if (!string.IsNullOrEmpty(gameStateJson))
                {
                    try
                    {
                        localGameState = JsonUtility.FromJson<GameState>(gameStateJson); // Пробую загрузить старый тип сохранения
                    }
                    catch
                    {
                        Debug.LogWarning("Can't load old save data, trying new");
                    }
                    finally
                    {
                        try
                        {
                            string json = ObscuredString.Decrypt(gameStateJson.ToCharArray(), AntiCheatManager.Hash);
                            localGameState = JsonUtility.FromJson<GameState>(json);
                        }
                        catch
                        {
                            Debug.LogWarning("Can't load new save data");
                        }
                    }
                }

                if (cloudGameState != null)
                {
                    if (localGameState == null || cloudGameState.PlayerState.PlaytimeSeconds >= localGameState.PlayerState.PlaytimeSeconds)
                    {
                        GameState = cloudGameState;
                        Debug.Log("Loaded cloud save");
                    }
                    else
                    {
                        GameState = localGameState;
                        Debug.Log("Loaded local save");
                    }
                }
                else
                {
                    if (localGameState == null)
                    {
                        newGame = true;
                    }
                    else
                    {
                        GameState = localGameState;
                        Debug.Log("Loaded local save");
                    }
                }

                CheckBoostersExpired(); // Проверяем истекли ли активные бустеры

                foreach (var rebirthUpgrade in RebirthUpgrades)
                {
                    if (GameState.PlayerState.RebirthUpgradeStates.Any(e => e.Id == rebirthUpgrade.Id))
                        continue;

                    GameState.PlayerState.RebirthUpgradeStates.Add(new RebirthUpgradeState
                    {
                        Id = rebirthUpgrade.Id,
                        Level = 0,
                    });
                }
            }
            else
            {
                newGame = true;
            }

            if (newGame)
            {
                Debug.Log("New save");
                GameState = NewGameState();
                UpdateGameShop();
            }
            else
            {
                GameState.PlayerState.Stats.RebirthCount = GameState.PlayerState.RebirthCount;
            }

            // Проверяем существуют ли в игре предметы которые в инвентаре, если нет то удаляем
            {
                ItemState[] itemStates = GameState.PlayerState.Items.ToArray();
                foreach (var item in itemStates)
                {
                    var itemData = Items.FirstOrDefault(e => e.Id == item.Id);
                    if (itemData != null)
                        continue;

                    GameState.PlayerState.Items.Remove(item);
                }
            }

            #region RemoteConfig
            var blockDestroyVisual = (string)FirebaseManager.Instance.RemoteConfig["block_destroy_visual"];
            GameState.GraphicsQuality = blockDestroyVisual == "default" ? GraphicsQuality.Low : GraphicsQuality.High;

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("type", blockDestroyVisual);

            AmplitudeManager.Instance.Event(AnalyticEventKey.START_GAME_GRAPHICS, properties);
            #endregion

            var playerState = GameState.PlayerState;
            var epsMultiplier = RebirthUpgrades.FirstOrDefault(e => e.Id == "eps_multiplier");
            var epsMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "eps_multiplier");

            ExperiencePerSecondMultiplier = 1f + ((epsMultiplier.Value * epsMultiplierState.Level) / 100f);

            float offlineTime = ServerTimeManager.Instance.ServerTime - playerState.LastSaveTimestamp;
            CalculateOfflineIncome(offlineTime);

            TryShowDailyReward();

            StartCoroutine(ExperiencePerSecond());
            StartCoroutine(CheckAchievements());

            ReportCaveLevel(playerState.CaveLevel);
            ReportRebirthCount(playerState.Stats.RebirthCount);

            CleverAdsSolutionsManager.Instance.NoAds = GameState.NoAds;
            CleverAdsSolutionsManager.Instance.EnableTimer = playerState.CaveStates.Count >= 2;

            AchievementManager.Instance.CheckAllConditions(this);

            SaveGame();
        }

        private GameState NewGameState()
        {
            // Создаем новую игру
            var gameState = new GameState();

            gameState.PlayerState = new PlayerState
            {
                CaveLevel = 1,
            };

            gameState.PlayerState.CurrentCave = "C001";
            gameState.PlayerState.CaveStates.Add(new CaveState
            {
                Id = "C001",
                Level = 1,
            });

            //gameState.PlayerState.Items.Add(new ItemState
            //{
            //    Id = "IF001",
            //    Count = 12,
            //    CustomValue = "100%",
            //});

            //gameState.PlayerState.Items.Add(new ItemState
            //{
            //    Id = "IF003",
            //    Count = 12,
            //    CustomValue = "100%",
            //});

            foreach (var rebirthUpgrade in RebirthUpgrades)
            {
                gameState.PlayerState.RebirthUpgradeStates.Add(new RebirthUpgradeState
                {
                    Id = rebirthUpgrade.Id,
                    Level = 0,
                });
            }

            gameState.PlayerState.PickaxeState = new PickaxeState
            {
                DamageLevel = 1,
                WealthLevel = 1,
                ExperienceLevel = 1,
                AutoClickLevel = 0,
                Runes = new PickaxeRuneState[3],
            };

            return gameState;
        }

        public void GenerateNewCaveTasks(string caveId, int count)
        {
            var cave = Caves.FirstOrDefault(e => e.Id == caveId);

            if (cave == null)
                return;

            var caveTasks = Tasks.Where(e => e.TaskCategory == TaskCategory.Cave).ToList();

            if (caveTasks == null || caveTasks.Count <= 0)
                return;

            var playerState = GameState.PlayerState;

            for(int i = 0; i < count; i++)
            {
                int caveLevel = int.Parse(caveId.TrimStart('C'));
                int num = Random.Range(0, caveTasks.Count);

                bool isKeyReward = caveTasks[num].RewardType == TaskRewardType.CommonKey
                    || caveTasks[num].RewardType == TaskRewardType.RareKey
                    || caveTasks[num].RewardType == TaskRewardType.EpicKey
                    || caveTasks[num].RewardType == TaskRewardType.MythicalKey
                    || caveTasks[num].RewardType == TaskRewardType.LegendaryKey;

                var taskState = new TaskState
                {
                    Id = caveTasks[num].Id,
                    Category = caveId,
                    RewardValue = (int)(caveTasks[num].Reward * (isKeyReward ? 1f : Mathf.Pow(caveLevel, 2f))),
                };

                switch (caveTasks[num].TaskType)
                {
                    case TaskType.DestroyBlocks:
                        taskState.NeedValue = cave.GetRandomBlockData().Id;
                        taskState.TargetValue = (int)(caveTasks[num].TargetValue * caveLevel);
                        break;
                    case TaskType.DropKeys:
                        var keyType = (RarityType)Random.Range(0, Enum.GetNames(typeof(RarityType)).Length - 2);
                        taskState.TargetValue = Mathf.Clamp(taskState.TargetValue / (((int)keyType + 1) * 2), 1, int.MaxValue);
                        taskState.NeedValue = keyType.ToString();
                        break;
                    case TaskType.OpenChests:
                        var chestType = (RarityType)Random.Range(0, Enum.GetNames(typeof(RarityType)).Length - 2);
                        taskState.TargetValue = Mathf.Clamp(taskState.TargetValue / (((int)chestType + 1) * 2), 1, int.MaxValue);
                        taskState.NeedValue = chestType.ToString();
                        break;
                    case TaskType.ClickCount:
                        taskState.TargetValue = (int)(caveTasks[num].TargetValue * caveLevel);
                        break;
                }

                playerState.Tasks.Add(taskState);
            }
        }

        private void CalculateOfflineIncome(float offlineTime)
        {
            Debug.Log("OfflineTime: " + offlineTime);

            if (offlineTime <= 120f) // Минимальное оффлайн время для отображения окна
                return;

            var offlineIncomeState = new OfflineIncomeState();

            var playerState = GameState.PlayerState;

            var maxOfflineTimeRebirth = RebirthUpgrades.FirstOrDefault(e => e.Id == "max_offline_time");
            var maxOfflineTimeRebirthState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "max_offline_time");

            var offlineIncomeMultiplierRebirth = RebirthUpgrades.FirstOrDefault(e => e.Id == "offline_income_multiplier");
            var offlineIncomeMultiplierRebirthState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "offline_income_multiplier");

            float maxOfflineTime = GameData.DefaultMaxOfflineTime + ((maxOfflineTimeRebirth.Value * maxOfflineTimeRebirthState.Level) * 60);
            float offlineIncomeMultiplier = GameData.DefaultOfflineIncomeMultiplier + ((offlineIncomeMultiplierRebirth.Value * offlineIncomeMultiplierRebirthState.Level) / 100);

            offlineTime = Mathf.Clamp(offlineTime, 0, maxOfflineTime);

            offlineIncomeState.Experience += (int)((playerState.StorageState.ExperiencePerSecond * offlineTime) * offlineIncomeMultiplier); // учитываем EPS

            var currentCave = Caves.FirstOrDefault(e => e.Id == playerState.CurrentCave); // Текущая шахта

            var caveState = GetCaveState(playerState.CurrentCave);

            float averageHealthPerBlock = (float)currentCave.GetAverageHealthPerBlock(); // Среднее количество хп у блока
            float averageExperiencePerBlock = (float)currentCave.GetAveragePricePerBlock(caveState.Level); // Среднее количество опыта с блока

            var offlinePickaxeSpeedMultiplier = RebirthUpgrades.FirstOrDefault(e => e.Id == "offline_pickaxe_speed_multiplier");
            var offlinePickaxeSpeedMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "offline_pickaxe_speed_multiplier");

            float pickaxeSpeedMultiplier = Pickaxe.GetPickaxeMultiplier() * (1f +((offlinePickaxeSpeedMultiplier.Value * offlinePickaxeSpeedMultiplierState.Level) / 100f)); // мощность удара кирки

            float averageDamagePerBlock = CaveController.GetCurrentBlockDamage(pickaxeSpeedMultiplier, averageHealthPerBlock); // средний урон наносимый блоку

            int clickPerMinutes = Pickaxe.GetAutoClickPerMinute(); // удары киркой в минуту
            float clickPerSecond = 60f / (float)clickPerMinutes; // удар киркой в секунду

            int hitBlockCount = (int)(offlineTime / clickPerSecond); // Количество ударов сделанных киркой

            float allMakedDamage = hitBlockCount * averageDamagePerBlock; // Сколько кирка нанесла общего урона
            int hitCountToDestroyBlock = Mathf.CeilToInt((averageHealthPerBlock / averageDamagePerBlock)); // Количество ударов чтобы разрушить блок

            int blocksDestroyed = (int)(hitBlockCount / hitCountToDestroyBlock); // Сколько разрушено блоков

            offlineIncomeState.Experience += (int)((averageExperiencePerBlock * blocksDestroyed) * offlineIncomeMultiplier * ExperiencePerSecondMultiplier); // Опыт добытый с блоков

            float mineralDropChance = (Pickaxe.MoneyDropChance() * 0.01f); // Шанс выбить деньги с блока

            int wealthLevel = PickaxeState.WealthLevel; // Уровень богатства кирки
            int baseMoney = GameData.BaseDroppedMoneyCount; // Стандартное количество выпадаемых денег с блока
            int maxMoneyStepCount = Mathf.Clamp(Mathf.FloorToInt((float)(wealthLevel - 1) / 1.4f), 1, int.MaxValue); // Количество раз которое деньги может умножиться
            float chanceToMoreMoney = GameData.BaseChanceToMoreMoneyCountDropped + ((wealthLevel - 1) * GameData.MoreMoneyCountDroppedChanceMultiplierPerWealthLevel); // Шанс умножения денег

            int maxDropedMoney = (int)Mathf.Pow(2f, maxMoneyStepCount); // Максимальное количество денег которое может выпасть

            offlineIncomeState.Minerals += (int)((Mathf.Pow(chanceToMoreMoney * 0.01f, maxMoneyStepCount) * maxDropedMoney * blocksDestroyed * mineralDropChance) * offlineIncomeMultiplier); // выпавшие минералы с блока
            
            if(offlineIncomeState.Experience <= 0 && offlineIncomeState.Minerals <= 0 && blocksDestroyed <= 0) // Если ничего не было добыто
            {
                return;
            }

            foreach (var block in currentCave.BlocksDatas) // Выпавшие ресурсы с шахты
            {
                int count = Mathf.CeilToInt((blocksDestroyed * ((block.SpawnPercentage.y - block.SpawnPercentage.x) * 0.01f)) * offlineIncomeMultiplier);

                if (count <= 0)
                    continue;

                offlineIncomeState.Resources.Add(new ResourceItemState
                {
                    Id = block.BlockData.ResourceItemData.Id,
                    Count = count,
                });
            }

            var offlineCanDropKeyState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "offline_can_drop_key");
            if (offlineCanDropKeyState.Level >= 1) // Могут ли выпасть ключи
            {
                float keyDropChance = Pickaxe.KeyDropChance() * 0.01f;
                int droppedKeyCount = (int)((blocksDestroyed * keyDropChance) * offlineIncomeMultiplier);

                for (int i = 0; i < droppedKeyCount; i++)
                {
                    switch (Pickaxe.GetDroppedKey())
                    {
                        case RarityType.Common:
                            offlineIncomeState.CommonKey++;
                            break;
                        case RarityType.Rare:
                            offlineIncomeState.RareKey++;
                            break;
                        case RarityType.Epic:
                            offlineIncomeState.EpicKey++;
                            break;
                        case RarityType.Mythical:
                            offlineIncomeState.MythicalKey++;
                            break;
                        case RarityType.Legendary:
                            offlineIncomeState.LegendaryKey++;
                            break;
                    }
                }
            }

            // Засчитываем добытые вещи
            {
                AddKey(RarityType.Common, offlineIncomeState.CommonKey);
                AddKey(RarityType.Rare, offlineIncomeState.RareKey);
                AddKey(RarityType.Epic, offlineIncomeState.EpicKey);
                AddKey(RarityType.Mythical, offlineIncomeState.MythicalKey);
                AddKey(RarityType.Legendary, offlineIncomeState.LegendaryKey);
                AddMoney(offlineIncomeState.Minerals);
                AddExperience(offlineIncomeState.Experience);

                foreach (var resource in offlineIncomeState.Resources)
                    AddResourceItem(resource.Id, resource.Count);
            }

            _uiManager.OpenOfflineIncomePanel(offlineIncomeState);

            playerState.Stats.BlockDestroyed += blocksDestroyed;
        }

        public CaveState GetCaveState(string id)
        {
            if (!Caves.Any(e => e.Id == id))
                return null;

            var playerState = GameState.PlayerState;
            var caveState = playerState.CaveStates.FirstOrDefault(e => e.Id == id);

            if (caveState == null)
            {
                playerState.CaveStates.Add(new CaveState
                {
                    Id = id,
                    Level = 1,
                });
            }

            return playerState.CaveStates.FirstOrDefault(e => e.Id == id);
        }

        private void CheckBoostersExpired()
        {
            foreach (var activeBooster in GameState.PlayerState.BoosterStates.ToArray())
            {
                var booster = Items.FirstOrDefault(e => e.Id == activeBooster.Id) as BoosterItemData;

                if(booster == null)
                {
                    RemoveActiveBooster(activeBooster);
                    continue;
                }

                int timestampDiff = (int)ServerTimeManager.Instance.ServerTime - activeBooster.ActiveTimestamp;

                if(timestampDiff >= booster.ActiveTime) // Если таймер активного бустера прошел, деактивируем его
                {
                    RemoveActiveBooster(activeBooster);
                }
            }
        }

        public void SaveGame()
        {
            if (AntiCheatManager.Instance.CheatDetected)
                return;

            Debug.Log("Game Saved LOCAL");

            _time = 0f;
            GameState.PlayerState.LastSaveTimestamp = (int)ServerTimeManager.Instance.ServerTime;

            string encryptedJson = new string(ObscuredString.Encrypt(JsonUtility.ToJson(GameState), AntiCheatManager.Hash));
            ObscuredPrefs.Set<string>("game_state", encryptedJson);

            _currentSaveCount++;

            if (_currentSaveCount >= _saveCountToCloudSave)
            {
                _currentSaveCount = 0;

                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(encryptedJson);

#if !UNITY_EDITOR
                if (Social.localUser.authenticated)
                {
#if UNITY_ANDROID
                    GooglePlayGamesManager.Instance.SaveGame(jsonBytes, (int)GameState.PlayerState.PlaytimeSeconds);
#endif
                }
                else
                {
                    FirebaseManager.Instance.SaveGame(jsonBytes);
                }
#endif
            }
        }

        public void AddExperience(int count)
        {
            var playerState = GameState.PlayerState;
            playerState.Experience += count;

            onExperienceChanged?.Invoke(playerState.Experience);
        }

        public void AddCavePrize(int count)
        {
            CaveController.CaveState.PrizeCount += count;

            onCavePrizeChanged?.Invoke();
        }

        public void AddCaveDifficultyMultiplier(float multiplier)
        {
            CaveController.CaveState.DifficultyMultiplier += multiplier;

            onCaveDifficultyMultiplierChanged?.Invoke();
        }

        public void AddMoney(int count)
        {
            var playerState = GameState.PlayerState;
            playerState.Money += count;

            onMoneyChanged?.Invoke(playerState.Money);
        }

        public void AddRebirthPoints(int count)
        {
            var playerState = GameState.PlayerState;
            playerState.RebirthPoints += count;

            onRebirthPointsChanged?.Invoke(playerState.RebirthPoints);
        }

        public void AddStorageExperiencePerSecond(float count)
        {
            var storageState = GameState.PlayerState.StorageState;

            var maxExperiencePerSecond = GetStorageMaxExperiencePerSecond(storageState.Level);
            storageState.ExperiencePerSecond = Mathf.Clamp(storageState.ExperiencePerSecond + count, 0, maxExperiencePerSecond);

            onExperiencePerSecondChanged?.Invoke();
        }

        public void AddKey(RarityType type, int count)
        {
            if (count == 0)
                return;

            var playerState = GameState.PlayerState;

            switch (type)
            {
                case RarityType.Common:
                    playerState.CommonKeyCount += count;
                    break;
                case RarityType.Rare:
                    playerState.RareKeyCount += count;
                    break;
                case RarityType.Epic:
                    playerState.EpicKeyCount += count;
                    break;
                case RarityType.Mythical:
                    playerState.MythicalKeyCount += count;
                    break;
                case RarityType.Legendary:
                    playerState.LegendaryKeyCount += count;
                    break;
            }

            onKeyChanged?.Invoke();

#region KeyCountEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("CommonKeyCount", playerState.CommonKeyCount);
            properties.Add("RareKeyCount", playerState.RareKeyCount);
            properties.Add("EpicKeyCount", playerState.EpicKeyCount);
            properties.Add("MythicalKeyCount", playerState.MythicalKeyCount);
            properties.Add("LegendaryKeyCount", playerState.LegendaryKeyCount);

            AmplitudeManager.Instance.Event(AnalyticEventKey.KEY_COUNT, properties);
#endregion
        }

        public ItemState AddItem(string id, int count, string customValue)
        {
            var itemData = Items.FirstOrDefault(e => e.Id == id);

            if (itemData == null)
                return null;

            ItemState returnItem = null;

            var playerState = GameState.PlayerState;

            var currentItem = playerState.Items.FirstOrDefault(e => e.Id == id && e.CustomValue == customValue);

            if (currentItem == null)
            {
                playerState.Items.Add(new ItemState
                {
                    Id = id,
                    Count = count,
                    CustomValue = customValue,
                });

                returnItem = playerState.Items[playerState.Items.Count - 1];
            }
            else
            {
                currentItem.Count += count;
                returnItem = currentItem;
            }

            onItemAdded?.Invoke(returnItem);
            return returnItem;
        }

        public void AddResourceItem(string id, int count)
        {
            var itemData = ResourceItems.FirstOrDefault(e => e.Id == id);

            if (itemData == null)
                return;

            ResourceItemState returnItem = null;

            var playerState = GameState.PlayerState;
            var currentItem = playerState.ResourceItems.FirstOrDefault(e => e.Id == id);

            if (currentItem == null)
            {
                playerState.ResourceItems.Add(new ResourceItemState
                {
                    Id = id,
                    Count = count,
                });

                returnItem = playerState.ResourceItems[playerState.ResourceItems.Count - 1];
            }
            else
            {
                currentItem.Count += count;
                returnItem = currentItem;
            }

            onResourceItemAdded?.Invoke(returnItem);
        }

        public void UpdateGameShop(int updateTime = -1)
        {
            var playerState = GameState.PlayerState;
            if (playerState.GameShop == null)
            {
                playerState.GameShop = new GameShopState();
                playerState.GameShop.Items = new List<ShopItemState>();
            }

            var gameShop = playerState.GameShop;

            if (updateTime == -1)
                gameShop.LastUpdateTimestamp = (int)ServerTimeManager.Instance.ServerTime;
            else
                gameShop.LastUpdateTimestamp = updateTime;

            gameShop.Items = new List<ShopItemState>();

            var shopItems = Items.Where(e => e.ShopCountMax > 0).ToList();

            for (int i = 0; i < 3; i++)
            {
                var item = shopItems[Random.Range(0, shopItems.Count)];

                gameShop.Items.Add(new ShopItemState
                {
                    Id = item.Id,
                    Price = Random.Range(item.ShopPriceMin, item.ShopPriceMax + 1),
                    Count = Random.Range(item.ShopCountMin, item.ShopCountMax + 1),
                });

                switch (item.ItemType)
                {
                    case ItemType.Rune:
                        gameShop.Items[gameShop.Items.Count - 1].CustomValue = $"{Random.Range(1, GameData.MaxRuneChanceInShop)}%";
                        break;
                    case ItemType.Hammer:
                        gameShop.Items[gameShop.Items.Count - 1].CustomValue = $"{Random.Range(1, GameData.HammerMaxChanceInShop)}%";
                        break;
                }
            }

            SaveGame();
        }

        public void AddRebirthUpgradeLevel(string rebirthUpgradeId)
        {
            var playerState = GameState.PlayerState;
            var rebirthUpgradeState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == rebirthUpgradeId);

            if (rebirthUpgradeState == null)
                return;

            rebirthUpgradeState.Level++;

#region EPSMultiplier
            var epsMultiplier = RebirthUpgrades.FirstOrDefault(e => e.Id == "eps_multiplier");
            var epsMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "eps_multiplier");

            ExperiencePerSecondMultiplier = 1f + ((epsMultiplier.Value * epsMultiplierState.Level) / 100f);
#endregion

            onAddedRebirthUpgradeLevel?.Invoke(rebirthUpgradeId);
        }

        private void TryShowDailyReward()
        {
            var playerState = GameState.PlayerState;

            if (playerState.CaveStates.Count >= 2)
            {
                float dailyRewardTime = ServerTimeManager.Instance.ServerTime - playerState.LastDailyRewardTimestamp;
                if (dailyRewardTime > 86400) // если с последнего взятия ежедневной награды прошло 24 часа
                {
                    if (dailyRewardTime > 172800) // если прошло 2 дня и больше
                    {
                        playerState.DailyRewardStrike = 0;
                    }

                    _uiManager.ShowDailyRewardPanel();
                }
            }
        }

        public void UpgradeLevel()
        {
            int experienceToUpgrade = GetExperienceCountToNextLevel();

            var playerState = GameState.PlayerState;
            playerState.CaveLevel++;

            string newCaveId = Caves[playerState.CaveLevel - 1].Id;
            if (!playerState.CaveStates.Any(e => e.Id == newCaveId))
            {
                playerState.CaveStates.Add(new CaveState
                {
                    Id = newCaveId,
                    Level = 1,
                });
            }

            if(playerState.CaveLevel >= 5 && !GameState.IsRated)
            {
                _uiManager.ShowReviewPanel();
            }

            if(playerState.CaveStates.Count == 2)
            {
                TryShowDailyReward();
            }

            GenerateNewCaveTasks(newCaveId, 3);

            AddExperience(-experienceToUpgrade);

            SaveGame();

#region UpgradeCaveEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("CaveLevel", playerState.CaveLevel);
            properties.Add("PickaxeSpeed", playerState.PickaxeState.DamageLevel);
            properties.Add("PickaxeWealth", playerState.PickaxeState.WealthLevel);
            properties.Add("PickaxeExperience", playerState.PickaxeState.ExperienceLevel);
            properties.Add("PickaxeAutoClick", playerState.PickaxeState.AutoClickLevel);
            properties.Add("Money", playerState.Money);

            AmplitudeManager.Instance.Event(AnalyticEventKey.UPGRADE_CAVE, properties);
#endregion

#region ReachCaveEvent
            switch (playerState.CaveLevel)
            {
                case 5:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Reach_5_mine");
                    break;
                case 10:
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("Reach_10_mine");
                    break;
            }
#endregion

            if (GameState.PlayerState.CaveStates.Count >= 2)
            {
                CleverAdsSolutionsManager.Instance.EnableTimer = true;
                _uiManager.CaveScreen.CanShowGift = true;
            }

            ReportCaveLevel(playerState.CaveLevel);

            AchievementManager.Instance.CheckCaveUnlockedCondition(playerState);
        }

        public void UpgradeCaveLevel(string caveId)
        {
            var caveState = GetCaveState(caveId);

            if (caveState == null)
                return;

            caveState.Level++;

            onCaveLevelChanged?.Invoke(caveId);
        }

        public void RemoveItemFromInventory(string id, int count)
        {
            foreach(var item in GameState.PlayerState.Items)
            {
                if (id == item.Id)
                {
                    item.Count -= count;

                    if(item.Count <= 0)
                    {
                        GameState.PlayerState.Items.Remove(item);
                        break;
                    }
                }
            };
        }

        public void SelectCave(string id)
        {
            var playerState = GameState.PlayerState;
            playerState.CurrentCave = id;

            onCaveChanged?.Invoke();
        }

        public void AddActiveBooster(string id)
        {
            GameState.PlayerState.BoosterStates.Add(new BoosterState
            {
                Id = id,
                ActiveTimestamp = (int)ServerTimeManager.Instance.ServerTime,
            });

            onActiveBoostersChanged?.Invoke();
        }

        public void RemoveActiveBooster(string id)
        {
            var activeBooster = GameState.PlayerState.BoosterStates.FirstOrDefault(e => e.Id == id);

            if (activeBooster == null)
                return;

            GameState.PlayerState.BoosterStates.Remove(activeBooster);

            onActiveBoostersChanged?.Invoke();
        }

        public void RemoveActiveBooster(BoosterState activeBooster)
        {
            if (activeBooster == null)
                return;

            GameState.PlayerState.BoosterStates.Remove(activeBooster);

            onActiveBoostersChanged?.Invoke();
        }

        private void OnTutorialCompleted(string tutorialId)
        {
            if (GameState.PlayerState.CompletedTutorialsIds.Contains(tutorialId))
                return;

            GameState.PlayerState.CompletedTutorialsIds.Add(tutorialId);
            SaveGame();

            if (!_uiManager.HaveOverlayPanel() && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave && !TutorialController.Instance.TutorialPlaying)
            {
                if (tutorialId == "navigation_panel_tutorial" && !GameState.PlayerState.CompletedTutorialsIds.Contains("inventory_tutorial"))
                {
                    TutorialController.Instance.StartTutorial("inventory_tutorial");
                }
            }
        }

        public int GetPlayerKeyCount(RarityType type)
        {
            switch (type)
            {
                case RarityType.Common:
                    return GameState.PlayerState.CommonKeyCount;
                case RarityType.Rare:
                    return GameState.PlayerState.RareKeyCount;
                case RarityType.Epic:
                    return GameState.PlayerState.EpicKeyCount;
                case RarityType.Mythical:
                    return GameState.PlayerState.MythicalKeyCount;
                case RarityType.Legendary:
                    return GameState.PlayerState.LegendaryKeyCount;
            }

            return 0;
        }

        public void UpgradeStorage()
        {
            var storageState = GameState.PlayerState.StorageState;
            storageState.Level++;

            storageState.Level = Mathf.Clamp(storageState.Level, 0, GameData.MaxStorageLevel);

            onStorageLevelChanged?.Invoke();
        }

        public int GetStorageMaxExperiencePerSecond(int storageLevel)
        {
            var playerState = GameState.PlayerState;
            var storageCapacityMultiplier = RebirthUpgrades.FirstOrDefault(e => e.Id == "storage_capacity_multiplier");
            var storageCapacityMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "storage_capacity_multiplier");

            float storageMultiplier = 1f + ((storageCapacityMultiplier.Value * storageCapacityMultiplierState.Level) * 0.01f);

            return (int)((GameData.DefaultMaxExperiencePerSecond * Mathf.Pow(GameData.MultiplierMaxExperiencePerSecondPerLevel, storageLevel - 1)) * storageMultiplier);
        }

        public int GetStorageUpgradePrice(int storageLevel)
        {
            return (int)(GameData.StorageUpgradeDefaultPrice * Mathf.Pow(GameData.StorageUpgradePriceMultiplierPerLevel, storageLevel - 1));
        }

        public int GetExperienceCountToNextLevel()
        {
            int caveLevel = GameState.PlayerState.CaveLevel;
            return GameData.GetExperienceCountToNextLevel(caveLevel);
        }

        public int GetExperienceCountToNextRebirth()
        {
            int rebirthCount = GameState.PlayerState.Stats.RebirthCount;
            return GameData.GetExperienceCountToRebirth(rebirthCount);
        }

        public void ReportCaveLevel(int level)
        {
            if (AntiCheatManager.Instance.CheatDetected || Social.localUser.authenticated)
                return;

            Social.ReportScore(level, GPGSIds.leaderboard_cave_level, null);
        }

        public void ReportRebirthCount(int count)
        {
            if (AntiCheatManager.Instance.CheatDetected || Social.localUser.authenticated)
                return;

            Social.ReportScore(count, GPGSIds.leaderboard_rebirth_count, null);
        }
    }
}