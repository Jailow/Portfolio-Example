using CaveMiner.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections;
using CaveMiner.Helpers;

namespace CaveMiner
{
    public class CaveController : MonoBehaviour
    {
        private float _currentCaveMultiplier;
        private CaveData _currentCaveData;
        private float _currentDamage;
        private float _currentBlockHealth;

        private float _time;
        private float _autoClickTime;
        private bool _haveAutoClick;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private BlockBase _currentBlock;
        private BlockBase _nextBlock;
        private CavePoolManager _cavePool;
        private Tween _blockShakeTween;
        private bool _nextRichestBlock;
        private Tween _inventoryButtonTween;
        //private CaveObject[] _caveObjects;

        public Action onCaveChanged;
        public Action<BlockData> onBlockSpawned;
        public Action<BlockData> onBlockDestroyed;
        public Action<RarityType> onKeyDropped;

        public CaveState CaveState { get; private set; }
        public bool IsUsingDynamite { get; private set; }
        public BlockData NextBlockData { get; private set; }
        public BlockData CurrentBlockData { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _gameManager.Pickaxe.onAutoClickChanged += UpdateAutoClickSpeed;
            _gameManager.Pickaxe.onSpeedChanged += UpdateCurrentDamage;
            _gameManager.onCaveChanged += OnCaveChanged;
            _gameManager.onCaveLevelChanged += OnCaveLevelChanged;
            _gameManager.onGraphicsQualityChanged += (GraphicsQuality quality) => UpdateGraphicsQuality();

            //_caveObjects = GetComponentsInChildren<CaveObject>(true);

            _cavePool = new CavePoolManager(_uiManager.CaveScreen.BlocksParent, 4);

            var playerState = _gameManager.GameState.PlayerState;
            CaveState = playerState.CaveStates.FirstOrDefault(e => e.Id == playerState.CurrentCave);
            _currentCaveData = _gameManager.Caves.FirstOrDefault(e => e.Id == playerState.CurrentCave);
            _currentCaveMultiplier = _currentCaveData.GetExperienceMultiplier(_gameManager.GetCaveState(_currentCaveData.Id).Level);

            var blockImage = uiManager.CaveScreen.BlockImage;
            var blockEventTrigger = blockImage.GetComponent<EventTrigger>();

            EventTrigger.Entry pointerClick = new EventTrigger.Entry();
            pointerClick.eventID = EventTriggerType.PointerClick;
            pointerClick.callback.AddListener((clickEvent) => 
            {
                _gameManager.OnHitBlock();
                OnHit(clickEvent);
            });

            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener(OnPointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener(OnPointerUp);

            blockEventTrigger.triggers.Add(pointerClick);
            blockEventTrigger.triggers.Add(pointerDown);
            blockEventTrigger.triggers.Add(pointerUp);

            UpdateAutoClickSpeed();
            Invoke(nameof(UpdateGraphicsQuality), 0.02f);
        }

        private void Update()
        {
            if (!_haveAutoClick)
                return;

            if (Time.time >= _time)
            {
                _time = Time.time + _autoClickTime;
                OnHit(null);
            }
        }

        private void UpdateAutoClickSpeed()
        {
            int clickPerMinutes = _gameManager.Pickaxe.GetAutoClickPerMinute();
            _haveAutoClick = clickPerMinutes > 0;
            _autoClickTime = 60f / (float)clickPerMinutes;
        }

        private void OnCaveLevelChanged(string id)
        {
            if (id != _currentCaveData.Id)
                return;

            _currentCaveMultiplier = _currentCaveData.GetExperienceMultiplier(_gameManager.GetCaveState(_currentCaveData.Id).Level);
        }

        private IEnumerator HoldAutoHit()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var holdClickSpeedState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "hold_click_speed");

            if (holdClickSpeedState.Level <= 0)
                yield break;

            yield return new WaitForSeconds(0.25f);

            var holdClickSpeed = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "hold_click_speed");
            float clickTime = 60f / (holdClickSpeed.Value * holdClickSpeedState.Level);

            var waitTime = new WaitForSeconds(clickTime);

            var holdClickDamage = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "hold_click_damage");
            var holdClickDamageState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "hold_click_damage");
            float damageMultiplier = 1f + (holdClickDamage.Value * holdClickDamageState.Level * 0.01f);

            while (true)
            {
                OnHit(null, damageMultiplier);
                yield return waitTime;
            }
        }

        private void OnPointerDown(BaseEventData data)
        {
            StartCoroutine(HoldAutoHit());
        }

        private void OnPointerUp(BaseEventData data)
        {
            StopAllCoroutines();
        }

        private void OnHit(BaseEventData data, float damageMultiplier = 1f)
        {
            if(data != null)
            {
                _gameManager.GameState.PlayerState.Stats.ClickCount++;
            }

            if (_gameManager.GameState.SoundsIsOn && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
            {
                var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                audioShot.Play(_gameManager.SoundData.GetBlockHit(CurrentBlockData.Surface));
            }

            bool criticalDamage = _gameManager.Pickaxe.IsCriticalDamage();
            if (criticalDamage)
            {
                if (_gameManager.GameState.SoundsIsOn && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                {
                    var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                    audioShot.Play(_gameManager.SoundData.GetCriticalDamage());
                }

                var playerState = _gameManager.GameState.PlayerState;
                var criticalDamageMultiplier = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "critical_damage_multiplier");
                var criticalDamageMultiplierState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "critical_damage_multiplier");

                float multiplier = _gameManager.GameData.CriticalDamageMultiplier * (1f + (criticalDamageMultiplier.Value * criticalDamageMultiplierState.Level / 100f));

                MakeDamage(_currentDamage * multiplier * damageMultiplier);

                #region CriticalDamageVFX
                if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                {
                    var criticalDamageVFX = ObjectPoolManager.Instance.GetObject(PoolName.CriticalDamageVFX).GetComponent<ParticleSystemRenderer>();
                }
                #endregion
            }
            else
            {
                MakeDamage(_currentDamage * damageMultiplier);
            }

            _blockShakeTween?.Complete();

            if (_currentBlock != null)
                _blockShakeTween = _currentBlock.transform.DOShakePosition(0.1f, 25 * (criticalDamage ? 2f : 1f), 25 * (criticalDamage ? 2 : 1));

            _uiManager.CaveScreen.PickaxeAnimator.SetTrigger("Hit");
        }

        private void MakeDamage(float damage)
        {
            if (CurrentBlockData == null || _uiManager == null)
                return;

            _currentBlockHealth -= damage;

            float healthValue = Mathf.Clamp01(_currentBlockHealth / (CurrentBlockData.Health * CaveState.DifficultyMultiplier));
            _uiManager.NavigationPanel.HealthBar.Set(healthValue);

            if(_gameManager.GameState.GraphicsQuality == GraphicsQuality.High)
                _currentBlock?.SetDestroy(healthValue, false);

            if (_currentBlockHealth <= 0)
            {
                DestroyCurrentBlock();
            }
        }

        private void OnCaveChanged()
        {
            if (_currentBlock != null && !_currentBlock.IsDestroyed)
                _currentBlock.DestroyObject();

            if (_nextBlock != null && !_nextBlock.IsDestroyed)
                _nextBlock.DestroyObject();

            _currentBlock = null;
            _nextBlock = null;
            _nextRichestBlock = false;

            var playerState = _gameManager.GameState.PlayerState;
            CaveState = playerState.CaveStates.FirstOrDefault(e => e.Id == playerState.CurrentCave);
            _currentCaveData = _gameManager.Caves.FirstOrDefault(e => e.Id == playerState.CurrentCave);
            _currentCaveMultiplier = _currentCaveData.GetExperienceMultiplier(_gameManager.GetCaveState(_currentCaveData.Id).Level);

            var currentCave = _gameManager.Caves.FirstOrDefault(e => e.Id == playerState.CurrentCave);

            if (NextBlockData != null)
                NextBlockData = currentCave.GetRandomBlockData();

            onCaveChanged?.Invoke();

            SpawnRandomBlock();
        }

        private void PlayInventoryButtonAnimation()
        {
            _inventoryButtonTween?.Complete();
            _inventoryButtonTween = _uiManager.NavigationPanel.InventoryIconTr.DOShakeScale(0.12f, 0.15f, 5, 40, true, ShakeRandomnessMode.Harmonic);
        }

        public void DestroyCurrentBlock(bool canUseImpulseRuneChance = true)
        {
            var playerState = _gameManager.GameState.PlayerState;
            var currentCaveState = playerState.CaveStates.FirstOrDefault(e => e.Id == playerState.CurrentCave);

            playerState.Stats.BlockDestroyed++;
            currentCaveState.CurrentDepth++;

            if(currentCaveState.CurrentDepth % _gameManager.GameData.PriceEveryDepth == 0 && currentCaveState.CurrentDepth > 0) // Добавляем приз за пройденое расстояние
            {
                _gameManager.AddCavePrize(1);
            }

            if(currentCaveState.CurrentDepth % 10 == 0 && currentCaveState.CurrentDepth > 0) // Добавляем множитель сложности шахты за пройденое расстояние
            {
                _gameManager.AddCaveDifficultyMultiplier(_gameManager.GameData.CaveDifficultyMultiplierPerTenDepth);
            }

            #region Vibration
            if (_gameManager.GameState.VibrationIsOn && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
            {
                Vibration.Vibrate(30);
            }
            #endregion

            #region BreakBlockVFX
            if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave && playerState.CompletedTutorialsIds.Contains("inventory_tutorial"))
            {
                var cubeParticle = ObjectPoolManager.Instance.GetObject(PoolName.BreakBlockCube)?.GetComponent<CubeParticle>();
                if (cubeParticle != null)
                {
                    if (!cubeParticle.IsInitialized)
                        cubeParticle.Init(_gameManager, _uiManager);

                    cubeParticle.PlayAnimation(CurrentBlockData.BreakBlockParticleMaterial, _uiManager.CaveScreen.BlockParticleSpawnAnchor.position, _uiManager.NavigationPanel.InventoryIconTr.position, PlayInventoryButtonAnimation);
                }
            }
            #endregion

            switch (_gameManager.GameState.GraphicsQuality)
            {
                case GraphicsQuality.Low:
                    _currentBlock?.DestroyObject();
                    break;
                case GraphicsQuality.High:
                    _currentBlock?.SetDestroy(0f, IsUsingDynamite);
                    break;
            }

            int droppedExperience = (int)((CurrentBlockData.ExperienceCount * _currentCaveMultiplier) * _gameManager.Pickaxe.GetExperienceMultiplier());
            _gameManager.AddExperience(droppedExperience);

            var gameData = _gameManager.GameData;

            // Базовое обучение по интерфейсу
            {
                if (!TutorialController.Instance.TutorialPlaying/* && !playerState.CompletedTutorialsIds.Contains("rebirth_tutorial")*/)
                {
                    if (!_uiManager.HaveOverlayPanel() && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                    {
                        if (!playerState.CompletedTutorialsIds.Contains("top_panel_tutorial") && playerState.Experience >= 3)
                        {
                            TutorialController.Instance.StartTutorial("top_panel_tutorial");
                        }
                        else if (!playerState.CompletedTutorialsIds.Contains("main_buttons_tutorial") && playerState.Experience >= 11)
                        {
                            TutorialController.Instance.StartTutorial("main_buttons_tutorial");
                        }
                        else if (!playerState.CompletedTutorialsIds.Contains("cave_upgrade_tutorial") && playerState.Experience >= _gameManager.GetExperienceCountToNextLevel())
                        {
                            TutorialController.Instance.StartTutorial("cave_upgrade_tutorial");
                        }
                        else if (!playerState.CompletedTutorialsIds.Contains("upgrades_tutorial") && playerState.CompletedTutorialsIds.Contains("inventory_tutorial") && playerState.Money >= 10)
                        {
                            TutorialController.Instance.StartTutorial("upgrades_tutorial");
                        }
                        else if (!playerState.CompletedTutorialsIds.Contains("shop_tutorial") && playerState.Money >= 100)
                        {
                            TutorialController.Instance.StartTutorial("shop_tutorial");
                        }
                        else if(!playerState.CompletedTutorialsIds.Contains("tasks_tutorial") && playerState.Experience >= 250)
                        {
                            TutorialController.Instance.StartTutorial("tasks_tutorial");
                        }
                        else if (!playerState.CompletedTutorialsIds.Contains("rebirth_tutorial") && playerState.Experience >= 30000)
                        {
                            TutorialController.Instance.StartTutorial("rebirth_tutorial");
                        }
                    }
                }
            }

            bool keyPanelTutorialCompleted = playerState.CompletedTutorialsIds.Contains("key_panel_tutorial");

            if ((keyPanelTutorialCompleted && _gameManager.Pickaxe.KeyDropped()) || (!keyPanelTutorialCompleted && playerState.Experience >= 20 && !TutorialController.Instance.TutorialPlaying))
            {
                var droppedKey = _gameManager.Pickaxe.GetDroppedKey();
                _gameManager.AddKey(droppedKey, 1);

                AudioClip dropKeySound = null;

                switch (droppedKey)
                {
                    case RarityType.Common:
                        dropKeySound = _gameManager.SoundData.CommonKeyDrop;
                        if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                            _uiManager.CaveDropFlyingTexts.Show($"<color=#{gameData.KeyTextColor(droppedKey)}>+1 COMMON KEY");
                        break;
                    case RarityType.Rare:
                        dropKeySound = _gameManager.SoundData.CommonKeyDrop;
                        if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                            _uiManager.CaveDropFlyingTexts.Show($"<color=#{gameData.KeyTextColor(droppedKey)}>+1 RARE KEY");
                        break;
                    case RarityType.Epic:
                        dropKeySound = _gameManager.SoundData.CommonKeyDrop;
                        if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                            _uiManager.CaveDropFlyingTexts.Show($"<color=#{gameData.KeyTextColor(droppedKey)}>+1 EPIC KEY");

                        _gameManager.SaveGame();
                        break;
                    case RarityType.Mythical:
                        dropKeySound = _gameManager.SoundData.CommonKeyDrop;
                        if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                            _uiManager.CaveDropFlyingTexts.Show($"<color=#{gameData.KeyTextColor(droppedKey)}>+1 MYTHICAL KEY");

                        _gameManager.SaveGame();
                        break;
                    case RarityType.Legendary:
                        dropKeySound = _gameManager.SoundData.CommonKeyDrop;
                        if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                            _uiManager.CaveDropFlyingTexts.Show($"<color=#{gameData.KeyTextColor(droppedKey)}>+1 LEGENDARY KEY");
                        break;
                }

                if(dropKeySound != null && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                {
                    if (_gameManager.GameState.SoundsIsOn && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                    {
                        var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                        audioShot.Play(dropKeySound);
                    }

                    if (!TutorialController.Instance.TutorialPlaying)
                    {
                        if (!keyPanelTutorialCompleted)
                        {
                            if (!_uiManager.HaveOverlayPanel() && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                                TutorialController.Instance.StartTutorial("key_panel_tutorial");
                        }
                        else if((int)droppedKey >= 1 && !playerState.CompletedTutorialsIds.Contains("change_key_tutorial"))
                        {
                            if (!_uiManager.HaveOverlayPanel() && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                                TutorialController.Instance.StartTutorial("change_key_tutorial");
                        }
                    }
                }

                onKeyDropped?.Invoke(droppedKey);
            }
            else
            {
                if (_gameManager.Pickaxe.MoneyDropped())
                {
                    int moneyDropCount = _gameManager.Pickaxe.GetDroppedMoneyCount();

                    if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                    {
                        if (_uiManager.CaveDropFlyingTexts.Show($"<color=#{gameData.MoneyTextColor}>+{Helpers.NumberToString.Convert(moneyDropCount)} MINERALS"))
                        {
                            if (_gameManager.GameState.SoundsIsOn)
                            {
                                var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                                audioShot.Play(_gameManager.SoundData.MineralsDrop);
                            }
                        }
                    }

                    _gameManager.AddMoney(moneyDropCount);
                }
            }

            if (CurrentBlockData.ResourceItemData != null) // получаем ресурс с блока
            {
                _gameManager.AddResourceItem(CurrentBlockData.ResourceItemData.Id, 1);
            }

            onBlockDestroyed?.Invoke(CurrentBlockData);

            SpawnRandomBlock();

            if (canUseImpulseRuneChance)
            {
                var impulseRuneState = playerState.PickaxeState.Runes.FirstOrDefault(e => e.Id == "IF003"); // с учётом руны ИМПУЛЬС
                //impulseRuneState = new PickaxeRuneState
                //{
                //    Id = "IF003",
                //    Level = 14,
                //};

                if (impulseRuneState != null)
                {
                    var activateRuneChance = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "activate_rune_chance");
                    var activateRuneChanceState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "activate_rune_chance");

                    var impulseRuneItem = _gameManager.Items.FirstOrDefault(e => e.Id == impulseRuneState.Id) as RuneItemData;
                    float activateChance = impulseRuneItem.GetFirstValue(impulseRuneState.Level);

                    //activateChance = 100;
                    if (Random.Range(0f, 100f) <= (activateChance + (activateRuneChance.Value * activateRuneChanceState.Level)))
                    {
                        if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                        {
                            ObjectPoolManager.Instance.GetObject(PoolName.ImpulseVFX);
                        }

                        int destroyBlockCount = (int)impulseRuneItem.GetSecondValue(impulseRuneState.Level);

                        IsUsingDynamite = true;

                        for (int i = 0; i < destroyBlockCount; i++)
                            DestroyCurrentBlock(false);

                        IsUsingDynamite = false;

                        SpawnRandomBlock();
                    }
                }
            }
        }

        private void UpdateCurrentDamage()
        {
            float pickaxeSpeedMultiplier = _gameManager.Pickaxe.GetPickaxeMultiplier();
            _currentDamage = GetCurrentBlockDamage(pickaxeSpeedMultiplier, CurrentBlockData.Health);
        }

        public void UseDynamite(int count)
        {
            IsUsingDynamite = true;

            if(_gameManager.GameState.GraphicsQuality == GraphicsQuality.High)
                _currentBlock.SetDestroy(1f, true);

            for (int i = 0; i < count; i++)
                DestroyCurrentBlock(false);

            IsUsingDynamite = false;

            SpawnRandomBlock();
        }

        private void UpdateGraphicsQuality()
        {
            switch (_gameManager.GameState.GraphicsQuality)
            {
                case GraphicsQuality.Low:
                    _currentBlock?.ResetObject();
                    break;
                case GraphicsQuality.High:
                    if (_currentCaveData != null && CurrentBlockData != null && CurrentBlockData.BlockPrefab != null)
                    {
                        if(_currentBlock == null)
                            _currentBlock = _cavePool.GetBlockData(_currentCaveData.Id, CurrentBlockData);

                        float healthValue = Mathf.Clamp01(_currentBlockHealth / CurrentBlockData.Health);
                        _currentBlock.SetDestroy(healthValue, false);
                    }
                    break;
            }
        }

        public void SpawnRandomBlock()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var currentCave = _gameManager.Caves.FirstOrDefault(e => e.Id == playerState.CurrentCave);

            var treasuryRuneState = playerState.PickaxeState.Runes.FirstOrDefault(e => e.Id == "IF004"); // с учётом руны СОКРОВИЩНИЦЫ
            //treasuryRuneState = new PickaxeRuneState
            //{
            //    Id = "IF004",
            //    Level = 7,
            //};

            if (NextBlockData == null)
                NextBlockData = currentCave.GetRandomBlockData();

            CurrentBlockData = NextBlockData;

            if (treasuryRuneState != null)
            {
                if (_nextRichestBlock)
                {
                    if (_gameManager.GameState.SoundsIsOn && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                    {
                        var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                        audioShot.Play(_gameManager.SoundData.TreasuryRune);
                    }

                    #region TreasuryRuneVFX
                    if (_uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                    {
                        var treasuryRuneVFX = ObjectPoolManager.Instance.GetObject(PoolName.TreasuryRuneVFX).GetComponent<ParticleSystemRenderer>();
                    }
                    #endregion
                }

                var activateRuneChance = _gameManager.RebirthUpgrades.FirstOrDefault(e => e.Id == "activate_rune_chance");
                var activateRuneChanceState = playerState.RebirthUpgradeStates.FirstOrDefault(e => e.Id == "activate_rune_chance");

                var impulseRuneItem = _gameManager.Items.FirstOrDefault(e => e.Id == treasuryRuneState.Id) as RuneItemData;
                float activateChance = impulseRuneItem.GetFirstValue(treasuryRuneState.Level);

                if (Random.Range(0f, 100f) <= (activateChance + (activateRuneChance.Value * activateRuneChanceState.Level)))
                {
                    _nextRichestBlock = true;
                    NextBlockData = currentCave.GetRichestExperienceBlockData();
                }
                else
                {
                    _nextRichestBlock = false;
                    NextBlockData = currentCave.GetRandomBlockData();
                }
            }
            else
            {
                _nextRichestBlock = false;
                NextBlockData = currentCave.GetRandomBlockData();
            }

            _currentBlockHealth = CurrentBlockData.Health * CaveState.DifficultyMultiplier;

            UpdateCurrentDamage();

            _uiManager.NavigationPanel.HealthBar.Set(1f);

            onBlockSpawned?.Invoke(CurrentBlockData);

            if (CurrentBlockData.BlockPrefab == null || IsUsingDynamite)
                return;

            if (_nextBlock == null)
            {
                _currentBlock = _cavePool.GetBlockData(_currentCaveData.Id, CurrentBlockData);
            }
            else
            {
                _currentBlock = _nextBlock;
            }

            var currentBlockPos = _currentBlock.Transform.localPosition;
            currentBlockPos.z = -20;
            _currentBlock.Transform.localPosition = currentBlockPos;

            _nextBlock = _cavePool.GetBlockData(_currentCaveData.Id, NextBlockData);

            var nextBlockPos = _nextBlock.Transform.localPosition;
            nextBlockPos.z = 125;
            _nextBlock.Transform.localPosition = nextBlockPos;
        }

        public float GetCurrentBlockDamage(float pickaxeMultiplier, float blockHealth)
        {
            float damage = pickaxeMultiplier / blockHealth;
            damage /= 30;
            float ticks = Mathf.CeilToInt(1f / damage);
            float seconds = ticks / 20; // время ломания блока в секундах

            damage = (1f / seconds) * blockHealth;
            damage /= 3;

            return damage;
        }
    }
}